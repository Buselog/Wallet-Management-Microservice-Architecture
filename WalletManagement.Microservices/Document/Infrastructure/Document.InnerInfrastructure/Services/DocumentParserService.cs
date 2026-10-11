using Document.Application.Dtos;
using Document.Application.Exceptions;
using Document.Application.Services;
using Microsoft.Extensions.Caching.Memory;

namespace Document.InnerInfrastructure.Services
{
    public class DocumentParserService : IDocumentParserService
    {
        private readonly IDocumentOcrClient _ocrClient;
        private readonly IWalletClient _walletClient;
        private readonly IMemoryCache _memoryCache;

        public DocumentParserService(IDocumentOcrClient ocrClient, IWalletClient walletClient, IMemoryCache memoryCache)
        {
            _ocrClient = ocrClient;
            _walletClient = walletClient;
            _memoryCache = memoryCache;
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

                string? draftToken = null;
                if (preCheckResult.CanBePaid)
                {
                    draftToken = Guid.NewGuid().ToString("N");

                    var draftModel = new InvoiceDraftModel
                    {
                        WalletId = walletId,
                        Amount = extractionResult.TotalAmount,
                        Currency = extractionResult.Currency,
                        ReferenceId = extractionResult.InvoiceNumber,
                        BillerName = extractionResult.BillerName
                    };

                    _memoryCache.Set($"invoice_draft:{draftToken}", draftModel, TimeSpan.FromMinutes(5));

                }

                preview.CanBePaid = preCheckResult.CanBePaid;
                preview.IsAlreadyPaid = preCheckResult.IsAlreadyPaid;
                preview.IsCurrencyMatched = preCheckResult.IsCurrencyMatched;
                preview.HasSufficientBalance = preCheckResult.HasSufficientBalance;

                if (!preCheckResult.IsCurrencyMatched)
                {
                    preview.ValidationParameters = new object[] { preview.Currency };
                }

                preview.ValidationMessage = preCheckResult.FailureReason;
                preview.DraftToken = draftToken;
            }
            catch (Exception ex)
            {
                preview.CanBePaid = false;
                preview.ValidationMessage = ex.Message;
            }

            return preview;
        }

        public async Task<bool> ConfirmAndPayAsync(string draftToken, CancellationToken cancellationToken = default)
        {
            string cacheKey = $"invoice_draft:{draftToken}";

            if (!_memoryCache.TryGetValue(cacheKey, out InvoiceDraftModel? draft) || draft == null)
            {
                throw new BaseBusinessException("ERR_DRAFT_EXPIRED_OR_INVALID");
            }

            _memoryCache.Remove(cacheKey);

            var payRequest = new PayInvoiceRequestDto
            {
                WalletId = draft.WalletId,
                Amount = draft.Amount,
                Currency = draft.Currency,
                ReferenceId = draft.ReferenceId,
                BillerName = draft.BillerName
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