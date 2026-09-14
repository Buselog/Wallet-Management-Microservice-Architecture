using Document.Application.Dtos;

namespace Document.Application.Services
{
    public interface IDocumentParserService
    {
        Task<InvoiceExtractionResultDto> ParseInvoiceAsync(Stream fileStream, string fileName, int walletId);
    }
}
