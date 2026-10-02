using Document.Application.Dtos;

namespace Document.Application.Services
{
    public interface IDocumentParserService
    {
        Task<InvoicePreviewResponseDto> ExtractAndPreviewAsync(
            Stream fileStream,
            string fileName,
            int walletId,
            CancellationToken cancellationToken = default);

        Task<bool> ConfirmAndPayAsync(
            ConfirmInvoicePaymentRequestDto request,
            CancellationToken cancellationToken = default);
    }
}
