using Wallet.Application.Dtos;
using Wallet.Domain.Entities.Enums;
using WalletEntity = Wallet.Domain.Entities.Concretes.Wallet;

namespace Wallet.Application.Managers
{
    public interface IWalletManager : IBaseManager<WalletEntity, WalletDto>
    {
        Task<decimal> GetBalanceAsync(int walletId, string customerNo);
        Task<List<WalletDto>> GetWalletsByCustomerNoAsync(string customerNo);
        Task<WalletDto> CreateNewWalletAsync(string customerNo, string currency, WalletType type);
        Task<string> DepositAsync(DepositRequestDto dto, string customerNo);
        Task<string> WithdrawAsync(WithdrawRequestDto dto, string customerNo);
        Task<string> TransferAsync(TransferRequestDto dto, string customerNo);
        Task<string> SoftDeleteWalletAsync(int walletId, string customerNo);
    }
}
