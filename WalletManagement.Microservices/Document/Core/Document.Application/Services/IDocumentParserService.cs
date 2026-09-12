
using Document.Application.Dtos;
using Microsoft.AspNetCore.Http;

namespace Document.Application.Services
{
    public interface IDocumentParserService
    {
        Task<InvoiceExtractionResultDto> ParseInvoiceAsync(IFormFile file, int walletId);
    }
}
