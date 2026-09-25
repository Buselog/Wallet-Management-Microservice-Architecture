using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Document.Application.Dtos;
using Document.Application.Services;
using Microsoft.Extensions.Configuration;

namespace Document.InnerInfrastructure.Services;

public class OpenRouterOcrClient : IDocumentOcrClient
{
    private readonly HttpClient _httpClient;
    private readonly IDocumentNormalizer _normalizer;
    private readonly string[] _models;
    private readonly int _perModelTimeout;

    public OpenRouterOcrClient(
        HttpClient httpClient,
        IConfiguration configuration,
        IDocumentNormalizer normalizer)
    {
        _httpClient = httpClient;
        _normalizer = normalizer;

        var apiKey = configuration["OpenRouterSettings:ApiKey"] ?? string.Empty;
        var baseUrl = configuration["OpenRouterSettings:BaseUrl"] ?? "https://openrouter.ai/api/v1/";

        var timeoutStr = configuration["OpenRouterSettings:PerModelTimeoutSeconds"];
        _perModelTimeout = int.TryParse(timeoutStr, out var t) ? t : 15;

        var modelSection = configuration.GetSection("OpenRouterSettings:Models");
        _models = modelSection.GetChildren()
            .Select(c => c.Value!)
            .Where(v => !string.IsNullOrEmpty(v))
            .ToArray();

        if (_models.Length == 0)
        {
            _models = new[] { "google/gemma-4-26b-a4b-it:free" };
        }

        _httpClient.BaseAddress = new Uri(baseUrl);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "http://localhost");
        _httpClient.DefaultRequestHeaders.Add("X-Title", "Document-Microservice-OCR");
    }

    public async Task<InvoiceExtractionResultDto> ExtractInvoiceDataAsync(Stream fileStream, string fileName)
    {
        var (imageBytes, mimeType) = await _normalizer.NormalizeToImageAsync(fileStream, fileName);
        var base64Data = Convert.ToBase64String(imageBytes);

        var attemptErrors = new List<string>();

        foreach (var model in _models)
        {
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_perModelTimeout));
                var payload = CreatePayloadForSingleModel(model, base64Data, mimeType);

                var response = await _httpClient.PostAsJsonAsync("chat/completions", payload, cts.Token);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync(cts.Token);
                    attemptErrors.Add($"{model} -> Status: {response.StatusCode} Detail: {errorBody}");
                    continue;
                }

                var responseJson = await response.Content.ReadAsStringAsync(cts.Token);
                var parsedResult = ParseModelResponse(responseJson, model);

                if (parsedResult != null && parsedResult.IsReceiptOrInvoice)
                {
                    return parsedResult;
                }
            }
            catch (Exception ex)
            {
                attemptErrors.Add($"{model} -> {ex.Message}");
            }
        }

        throw new HttpRequestException($"OCR modelleri başarısız oldu: {string.Join(" || ", attemptErrors)}");
    }

    private static object CreatePayloadForSingleModel(string modelName, string base64Data, string mimeType)
    {
        var prompt = @"Analyze this document carefully as a financial receipt/invoice parser:
1. isReceiptOrInvoice: Set to true if the document is a valid bill, commercial invoice, utility bill, or payment receipt. Otherwise false.
2. billerName: Official legal registered business name issuing the document (e.g., 'Ticaret Unvanı', ending with A.Ş., Ltd. Şti., etc.). Only if completely absent, use the visible commercial brand name.
3. invoiceNumber: The PRIMARY legal identifier of this document. Follow this strict priority order:
   a) Official GİB 16-character e-Invoice/e-Archive number (e.g. labeled as 'Belge No' or 'Fatura No' starting with 3 letters like GIB, DM0 followed by year and digits).
   b) If not present, the explicit 'Fatura No' or 'Receipt No / Fiş No'.
   c) Do NOT use Order Number, Tax ID, or ETTN.
4. totalAmount: The FINAL grand total payable amount including all taxes. Return strictly as a numeric decimal/float.
5. issueDate: The invoice issue/transaction date in ISO 'YYYY-MM-DD' format. If absent, use today's date.

Return ONLY a single valid raw JSON object matching this schema, without any markdown formatting or ```json code blocks:
{
  ""isReceiptOrInvoice"": true,
  ""billerName"": ""Company Name"",
  ""invoiceNumber"": ""DM02018003244118"",
  ""totalAmount"": 1869.90,
  ""issueDate"": ""YYYY-MM-DD""
}";

        return new
        {
            model = modelName,
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = new object[]
                    {
                        new { type = "text", text = prompt },
                        new
                        {
                            type = "image_url",
                            image_url = new { url = $"data:{mimeType};base64,{base64Data}" }
                        }
                    }
                }
            },
            temperature = 0.1
        };
    }

    private static InvoiceExtractionResultDto? ParseModelResponse(string jsonResponse, string modelName)
    {
        using var doc = JsonDocument.Parse(jsonResponse);
        var contentText = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        if (string.IsNullOrWhiteSpace(contentText)) return null;

        var cleanJson = contentText.Trim();
        if (cleanJson.StartsWith("```json")) cleanJson = cleanJson.Substring(7);
        if (cleanJson.StartsWith("```")) cleanJson = cleanJson.Substring(3);
        if (cleanJson.EndsWith("```")) cleanJson = cleanJson.Substring(0, cleanJson.Length - 3);
        cleanJson = cleanJson.Trim();

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<InvoiceExtractionResultDto>(cleanJson, options);

        if (result != null)
        {
            result.ExtractedBy = $"OpenRouter Fallback ({modelName})";
        }

        return result;
    }
}