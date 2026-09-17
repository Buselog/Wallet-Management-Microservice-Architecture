using Document.Application.Dtos;
using Document.Application.Exceptions;
using Document.Application.Services;


namespace Document.InnerInfrastructure.Services
{
    public class DocumentParserService : IDocumentParserService
    {
        private readonly IDocumentOcrClient _ocrClient;
        private readonly IWalletClient _walletClient;

        public DocumentParserService(IDocumentOcrClient ocrClient, IWalletClient walletClient)
        {
            _ocrClient = ocrClient;
            _walletClient = walletClient;
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

            var withdrawRequest = new WalletTransactionRequestDto
            {
                WalletId = walletId,
                Amount = extractionResult.TotalAmount,
                ReferenceId = extractionResult.InvoiceNumber ?? Guid.NewGuid().ToString()
            };

            await _walletClient.DeductBalanceAsync(withdrawRequest);

            return extractionResult;
        }
    }
}
