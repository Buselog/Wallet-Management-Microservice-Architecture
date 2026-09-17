using Document.Application.Dtos;

namespace Document.Application.Services
{
    public interface IWalletClient
    {
        Task<bool> DeductBalanceAsync(WalletTransactionRequestDto request);
    }
}
