using Document.Application.Dtos;
using Document.Application.Exceptions;
using Document.Application.Services;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using UglyToad.PdfPig;

namespace Document.InnerInfrastructure.Services;

public class OpenRouterOcrClient : IDocumentOcrClient
{
    private readonly HttpClient _httpClient;
    private readonly IDocumentNormalizer _normalizer;
    private readonly string[] _textModels;
    private readonly string[] _visionModels;
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
        _perModelTimeout = int.TryParse(timeoutStr, out var t) ? t : 18;

        _textModels = configuration.GetSection("OpenRouterSettings:TextModels")
            .GetChildren().Select(c => c.Value!).Where(v => !string.IsNullOrEmpty(v)).ToArray();

        _visionModels = configuration.GetSection("OpenRouterSettings:VisionModels")
            .GetChildren().Select(c => c.Value!).Where(v => !string.IsNullOrEmpty(v)).ToArray();

        if (_textModels.Length == 0)
        {
            _textModels = new[] { "google/gemma-4-26b-a4b-it:free", "google/gemma-4-31b-it:free" };
        }

        if (_visionModels.Length == 0)
        {
            _visionModels = new[] { "dots-studio/dots-3-note-preview:free" };
        }

        _httpClient.BaseAddress = new Uri(baseUrl);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "http://localhost");
        _httpClient.DefaultRequestHeaders.Add("X-Title", "Document-Microservice-OCR");
    }

    public async Task<InvoiceExtractionResultDto> ExtractInvoiceDataAsync(Stream fileStream, string fileName)
    {
        if (fileStream.CanSeek) fileStream.Position = 0;

        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        if (extension == ".pdf")
        {
            var extractedText = ExtractTextFromPdf(fileStream);

            if (!string.IsNullOrWhiteSpace(extractedText))
            {
                return await ProcessWithFallbackAsync(_textModels, model => CreateTextPayload(model, extractedText));
            }
        }

        var (imageBytes, mimeType) = await _normalizer.NormalizeToImageAsync(fileStream, fileName);
        var base64Data = Convert.ToBase64String(imageBytes);

        return await ProcessWithFallbackAsync(_visionModels, model => CreateVisionPayload(model, base64Data, mimeType));
    }

    private static string ExtractTextFromPdf(Stream fileStream)
    {
        try
        {
            using var document = PdfDocument.Open(fileStream);
            var sb = new StringBuilder();

            foreach (var page in document.GetPages())
            {
                sb.AppendLine(page.Text);
            }

            return sb.ToString().Trim();
        }
        catch
        {
            return string.Empty;
        }
    }

    private async Task<InvoiceExtractionResultDto> ProcessWithFallbackAsync(string[] models, Func<string, object> payloadFactory)
    {
        var attemptErrors = new List<string>();

        for (int i = 0; i < models.Length; i++)
        {
            var model = models[i];
            var isFallback = i > 0;

            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_perModelTimeout));
                var payload = payloadFactory(model);

                var response = await _httpClient.PostAsJsonAsync("chat/completions", payload, cts.Token);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync(cts.Token);
                    attemptErrors.Add($"{model} -> Status: {response.StatusCode} Detail: {errorBody}");
                    continue;
                }

                var responseJson = await response.Content.ReadAsStringAsync(cts.Token);
                var parsedResult = ParseModelResponse(responseJson, model, isFallback);

                if (parsedResult != null)
                {
                    if (!parsedResult.IsReceiptOrInvoice)
                    {
                        throw new DocumentNotRecognizedException();
                    }

                    return parsedResult;
                }
            }
            catch (DocumentNotRecognizedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                attemptErrors.Add($"{model} -> {ex.Message}");
            }
        }

        throw new OcrProviderException(string.Join(" || ", attemptErrors));
    }

    private static object CreateTextPayload(string modelName, string documentText)
    {
        var prompt = GetPrompt();
        var fullMessage = $"{prompt}\n\nRAW INVOICE TEXT CONTENT:\n{documentText}";

        return new
        {
            model = modelName,
            messages = new[]
            {
                new { role = "user", content = fullMessage }
            },
            temperature = 0.1
        };
    }

    private static object CreateVisionPayload(string modelName, string base64Data, string mimeType)
    {
        var prompt = GetPrompt();

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

    private static string GetPrompt()
    {
        return @"Analyze this document carefully as a financial receipt/invoice parser:
1. isReceiptOrInvoice: Set to true if the document is a valid bill, commercial invoice, utility bill, or payment receipt. Otherwise false.
- If the document is NOT a valid receipt or invoice:
  Set 'isReceiptOrInvoice' to false, 'totalAmount' to null, and leave other fields as null.
2. billerName: The legal or commercial entity ISSUING/SELLING on this document.
   - Priority 1 (Corporations): Legal registered business name of the issuer/seller (typically ending with A.Ş., Ltd. Şti., San. ve Tic., Anonim Şirketi, Kollektif Şirketi, etc.).
   - Priority 2 (Sole Proprietorships / Şahıs Şirketi): Full personal legal name of the merchant/seller appearing at the very top issuer section, directly tied to the seller's VKN/TCKN and registered tax office.
   - Priority 3 (Retail Receipts): Visible prominent brand/store trade name if and only if a formal legal title is entirely absent.
   - STRICT NEGATIVE RULES:
     * NEVER extract the recipient/buyer. Completely ignore sections labeled 'SAYIN', 'Sayın', 'Sn.', 'ALICI', 'MÜŞTERİ', 'CUSTOMER', 'BILLED TO', or 'DELIVERY TO'.
     * NEVER use placeholder header words like 'SAYIN', 'ALICI', 'Fatura', or 'e-Arşiv'.
     * NEVER extract third-party platforms, marketplaces, or payment providers (e.g. Trendyol, Hepsiburada, İyzico, PayTR, Yurtiçi Kargo).
     * NEVER extract e-invoice integrators, software providers, or campaign sponsors (e.g. EDM Bilişim, Logo, Foriba, Sovos, TEMA).
3. invoiceNumber: The PRIMARY legal identifier of this document. Follow this strict priority order:
   a) Official GİB 16-character e-Invoice/e-Archive number (e.g. labeled as 'Belge No' or 'Fatura No' starting with 3 letters like GIB, DM0 followed by year and digits).
   b) If not present, the explicit 'Fatura No' or 'Receipt No / Fiş No'.
   c) Do NOT use Order Number, Tax ID, or ETTN.
4. totalAmount: The FINAL NET PAYABLE total amount after all discounts and including all taxes.
   - Look specifically for labels like 'Ödenecek Tutar',  'Amount to be Paid', 'Genel Toplam', 'Vergiler Dahil Toplam Tutar', 'Total Amount Including Taxes', or 'Grand Total'.
   - NEVER use the undiscounted subtotal ('Mal Hizmet Toplam Tutarı') if discounts/iskonto exist.
   - Always pick the final net amount to be actually paid by the customer. Return strictly as a numeric decimal/float.
5. currency: The 3-letter ISO 4217 currency code of the invoice (e.g. 'TRY', 'EUR', 'USD', 'GBP').
   - If the symbol is '₺', 'TL' or not explicitly specified, return 'TRY'.
   - If '$', return 'USD'. If '€', return 'EUR'. If '£', return 'GBP'.
6. issueDate: The invoice issue/transaction date in ISO 'YYYY-MM-DD' format. If absent, use today's date.

Return ONLY a single valid raw JSON object matching this schema, without any markdown formatting or ```json code blocks:
{
  ""isReceiptOrInvoice"": true,
  ""billerName"": ""D-MARKET ELEKTRONİK HİZMETLER VE TİCARET A.Ş."",
  ""invoiceNumber"": ""DM02018003244118"",
  ""totalAmount"": 1869.90,
  ""currency"": ""TRY"",
  ""issueDate"": ""2024-05-15""
}";
    }

    private static InvoiceExtractionResultDto? ParseModelResponse(string jsonResponse, string modelName, bool isFallback)
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
            result.ExtractedBy = isFallback
                ? $"OpenRouter Fallback ({modelName})"
                : $"OpenRouter Primary ({modelName})";
        }

        return result;
    }
}