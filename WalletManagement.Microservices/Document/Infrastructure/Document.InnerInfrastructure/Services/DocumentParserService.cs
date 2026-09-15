using Document.Application.Dtos;
using Document.Application.Exceptions;
using Document.Application.Services;


namespace Document.InnerInfrastructure.Services
{
    public class DocumentParserService : IDocumentParserService
    {
        private readonly IDocumentOcrClient _ocrClient;

        public DocumentParserService(IDocumentOcrClient ocrClient)
        {
            _ocrClient = ocrClient;
        }
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

            var extractionResult = await _ocrClient.ExtractInvoiceDataAsync(fileStream, fileName);

            return extractionResult;
        }
    }
}
