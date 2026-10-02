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

        public async Task<InvoicePreviewResponseDto> ExtractAndPreviewAsync(
            Stream fileStream,
            string fileName,
            int walletId,
            CancellationToken cancellationToken = default)
        {
            ValidateFile(fileStream, fileName);

            var extractionResult = await _ocrClient.ExtractInvoiceDataAsync(fileStream, fileName);

            if (!extractionResult.TotalAmount.HasValue || extractionResult.TotalAmount.Value <= 0)
            {
                throw new InvoiceAmountNotFoundException();
            }

            var preview = new InvoicePreviewResponseDto
            {
                InvoiceNumber = extractionResult.InvoiceNumber,
                BillerName = extractionResult.BillerName,
                TotalAmount = extractionResult.TotalAmount,
                Currency = extractionResult.Currency ?? "TRY",
                IssueDate = extractionResult.IssueDate
            };

            try
            {
                var preCheckRequest = new InvoicePreCheckRequestDto
                {
                    WalletId = walletId,
                    Amount = extractionResult.TotalAmount.Value,
                    Currency = preview.Currency,
                    ReferenceId = extractionResult.InvoiceNumber ?? string.Empty
                };

                var preCheckResult = await _walletClient.PreCheckInvoiceAsync(preCheckRequest);

                preview.CanBePaid = preCheckResult.CanBePaid;
                preview.IsAlreadyPaid = preCheckResult.IsAlreadyPaid;
                preview.IsCurrencyMatched = preCheckResult.IsCurrencyMatched;
                preview.HasSufficientBalance = preCheckResult.HasSufficientBalance;
                preview.ValidationMessage = preCheckResult.FailureReason;
            }
            catch (Exception ex)
            {
                preview.CanBePaid = false;
                preview.ValidationMessage = ex.Message;
            }

            return preview;
        }

        public async Task<bool> ConfirmAndPayAsync(
            ConfirmInvoicePaymentRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var payRequest = new PayInvoiceRequestDto
            {
                WalletId = request.WalletId,
                Amount = request.Amount,
                Currency = request.Currency,
                ReferenceId = request.ReferenceId,
                BillerName = request.BillerName
            };

            return await _walletClient.PayInvoiceAsync(payRequest);
        }

        private static void ValidateFile(Stream fileStream, string fileName)
        {
            if (fileStream == null || fileStream.Length == 0)
                throw new FileEmptyException();

            const long maxFileSize = 5 * 1024 * 1024;
            if (fileStream.Length > maxFileSize)
                throw new FileSizeExceededException();

            var allowedExtensions = new[] { ".pdf", ".png", ".jpg", ".jpeg" };
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new InvalidFileExtensionException();
        }
    }
}