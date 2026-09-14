using Document.Application.Dtos;
using Document.Application.Exceptions;
using Document.Application.Services;


namespace Document.InnerInfrastructure.Services
{
    public class DocumentParserService : IDocumentParserService
    {
        public async Task<InvoiceExtractionResultDto> ParseInvoiceAsync(Stream fileStream, string fileName, int walletId)
        {
            if (fileStream == null || fileStream.Length == 0)
            {
                throw new FileEmptyException();
            }

            const long maxFileSize = 5 * 1024 * 1024;
            if (fileStream.Length > maxFileSize)
            {
                throw new FileSizeExceededException();
            }

            var allowedExtensions = new[] { ".pdf", ".png", ".jpg", ".jpeg" };
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                throw new InvalidFileExtensionException();
            }

            await Task.Delay(150);

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
