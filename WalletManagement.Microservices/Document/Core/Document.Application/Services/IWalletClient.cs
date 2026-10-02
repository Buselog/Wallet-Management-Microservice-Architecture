using Document.Application.Dtos;

namespace Document.Application.Services
{
    public interface IWalletClient
    {
        Task<List<WalletDto>> GetUserWalletsAsync();
        Task<InvoicePreCheckResponseDto> PreCheckInvoiceAsync(InvoicePreCheckRequestDto request);
        Task<bool> PayInvoiceAsync(PayInvoiceRequestDto payRequest);
    }
}