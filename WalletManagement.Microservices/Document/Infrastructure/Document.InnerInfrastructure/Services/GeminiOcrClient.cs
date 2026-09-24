using Document.Application.Dtos;
using Document.Application.Services;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;

namespace Document.InnerInfrastructure.Services;

public class GeminiOcrClient : IDocumentOcrClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _primaryModel;
    private readonly string _fallbackModel;

    public GeminiOcrClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["GeminiSettings:ApiKey"] ?? string.Empty;
        _primaryModel = configuration["GeminiSettings:Model"] ?? "gemini-3.8-flash";
        _fallbackModel = configuration["GeminiSettings:FallbackModel"] ?? "gemini-3.6-flash";
    }

    public async Task<InvoiceExtractionResultDto> ExtractInvoiceDataAsync(Stream fileStream, string fileName)
    {
        if (fileStream.CanSeek)
        {
            fileStream.Position = 0;
        }

        using var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream);
        var base64Data = Convert.ToBase64String(memoryStream.ToArray());

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var mimeType = extension switch
        {
            ".pdf" => "application/pdf",
            ".png" => "image/png",
            ".jpeg" or ".jpg" => "image/jpeg",
            _ => "application/octet-stream"
        };

        var requestPayload = CreateGeminiPayload(base64Data, mimeType);

        var response = await ExecuteWithModelFallbackAsync(requestPayload);

        return await ParseResponseAsync(response);

    }

    private async Task<HttpResponseMessage> ExecuteWithModelFallbackAsync(object payload)
    {
       
        var primaryUrl = BuildEndpointUrl(_primaryModel);
        var response = await _httpClient.PostAsJsonAsync(primaryUrl, payload);

        if (!response.IsSuccessStatusCode)
        {
            var fallbackUrl = BuildEndpointUrl(_fallbackModel);
            response = await _httpClient.PostAsJsonAsync(fallbackUrl, payload);
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Gemini API error ({response.StatusCode}): {errorBody}");
        }

        return response;
    }

    private string BuildEndpointUrl(string model) =>
        $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={_apiKey}";

    private static object CreateGeminiPayload(string base64Data, string mimeType)
    {
        return new
        {
            contents = new[]
            {
                new
                {
                    parts = new object[]
                    {
                        new
                        {
                            text = @"Analyze this document carefully as a financial receipt/invoice parser:
                             1. isReceiptOrInvoice: Set to true if the document is a valid bill, commercial invoice, utility bill, or payment receipt. Otherwise false.
                             2. billerName: Official vendor, merchant, utility, or company name issuing the document.
                             3. invoiceNumber: The PRIMARY legal identifier of this document. Follow this strict priority order:
                                a) Official GİB 16-character e-Invoice/e-Archive number (e.g. labeled as 'Belge No' or 'Fatura No' starting with 3 letters like GIB, DM0 followed by year and digits).
                                b) If not present, the explicit 'Fatura No' or 'Receipt No / Fiş No'.
                                c) Do NOT use Order Number, Tax ID, or ETTN.
                             4. totalAmount: The FINAL grand total payable amount including all taxes. Return strictly as a numeric decimal/float.
                             5. issueDate: The invoice issue/transaction date in ISO 'YYYY-MM-DD' format. If absent, use today's date."
                        },
                        new
                        {
                            inline_data = new
                            {
                                mime_type = mimeType,
                                data = base64Data
                            }
                        }
                    }
                }
            },
            generationConfig = new
            {
                response_mime_type = "application/json",
                response_schema = new
                {
                    type = "OBJECT",
                    properties = new
                    {
                        isReceiptOrInvoice = new { type = "BOOLEAN" },
                        billerName = new { type = "STRING" },
                        invoiceNumber = new { type = "STRING" },
                        totalAmount = new { type = "NUMBER" },
                        issueDate = new { type = "STRING", description = "YYYY-MM-DD formatted date string" }
                    },
                    required = new[] { "isReceiptOrInvoice", "totalAmount", "billerName" }
                }
            }
        };
    }

    private static async Task<InvoiceExtractionResultDto> ParseResponseAsync(HttpResponseMessage response)
    {
        var responseJson = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(responseJson);
        var contentText = doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var parsedDto = JsonSerializer.Deserialize<InvoiceExtractionResultDto>(contentText ?? "{}", options);

        if (parsedDto != null)
        {
            parsedDto.ExtractedBy = "Gemini-Flash-Vision";
            return parsedDto;
        }

        return new InvoiceExtractionResultDto
        {
            IsReceiptOrInvoice = false,
            ExtractedBy = "Gemini-Flash-Vision"
        };
    }
}
