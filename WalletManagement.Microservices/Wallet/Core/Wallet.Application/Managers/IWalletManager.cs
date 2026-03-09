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
        Task<string> ProcessTransactionAsync(CreateTransactionDto transactionDto, string type);
    }
}
