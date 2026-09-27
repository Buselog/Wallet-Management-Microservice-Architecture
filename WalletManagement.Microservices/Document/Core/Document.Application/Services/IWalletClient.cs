using Document.Application.Dtos;

namespace Document.Application.Services
{
    public interface IWalletClient
    {
        Task<List<WalletDto>> GetUserWalletsAsync();

        Task<bool> PayInvoiceAsync(PayInvoiceRequestDto payRequest);
    }
}