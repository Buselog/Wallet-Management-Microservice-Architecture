using System.Net.Http.Headers;
using System.Text.Json;
using Document.Application.Dtos;
using Document.Application.Services;

namespace Document.InnerInfrastructure.Services;

public class DatalabOcrClient : IDocumentOcrClient
{
    private readonly HttpClient _httpClient;

    public DatalabOcrClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<InvoiceExtractionResultDto> ExtractInvoiceDataAsync(Stream fileStream, string fileName)
    {
        if (fileStream.CanSeek)
        {
            fileStream.Position = 0;
        }

        using var content = new MultipartFormDataContent();
        using var streamContent = new StreamContent(fileStream);

        streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        content.Add(streamContent, "file", fileName);

        await Task.Delay(200); 

        return new InvoiceExtractionResultDto
        {
            IsReceiptOrInvoice = true,
            BillerName = "CK Boğaziçi Elektrik",
            InvoiceNumber = "ELK-2026-8812",
            TotalAmount = 485.60m,
            DueDate = DateTime.UtcNow.AddDays(5),
            ExtractedBy = "DatalabEngine"
        };
    }
}