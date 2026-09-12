using Document.Application.Dtos;
using Document.Application.Services;
using Microsoft.AspNetCore.Http;

namespace Document.InnerInfrastructure.Services
{
    public class DocumentParserService : IDocumentParserService
    {
        public async Task<InvoiceExtractionResultDto> ParseInvoiceAsync(IFormFile file, int walletId)
        {
            await Task.Delay(100);

            return new InvoiceExtractionResultDto
            {
                IsReceiptOrInvoice = true,
                BillerName = "İSKİ",
                InvoiceNumber = "FAT-2026-001",
                TotalAmount = 350.75m,
                DueDate = DateTime.UtcNow.AddDays(7),
                ExtractedBy = "MockEngine"
            };
        }
    }
}
