using Document.Application.Dtos;

namespace Document.Application.Services
{
    public interface IDocumentOcrClient
    {
        Task<InvoiceExtractionResultDto> ExtractInvoiceDataAsync(Stream fileStream, string fileName);
    }
}
