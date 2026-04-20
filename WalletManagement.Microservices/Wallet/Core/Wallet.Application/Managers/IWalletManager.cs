using Wallet.Application.Dtos;
using Wallet.Domain.Entities.Enums;
using WalletEntity = Wallet.Domain.Entities.Concretes.Wallet;

namespace Wallet.Application.Managers
{
    public interface IWalletManager : IBaseManager<WalletEntity, WalletDto>
    {
        Task<decimal> GetBalanceAsync(int walletId, string customerNo);
        Task<List<WalletDto>> GetWalletsByCustomerNoAsync(string customerNo);
        Task<WalletDto> CreateNewWalletAsync(string currentCustomerNo, string currency, WalletType? type);
        Task DepositAsync(DepositRequestDto dto, string customerNo);
        Task WithdrawAsync(WithdrawRequestDto dto, string customerNo);
        Task TransferAsync(TransferRequestDto dto, string customerNo);
        Task SoftDeleteWalletAsync(int walletId, string customerNo);
        Task ExecuteTradeAsync(CurrencyTradeRequestDto dto);
    }
}
