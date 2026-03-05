using Wallet.Application.Dtos;
using WalletEntity = Wallet.Domain.Entities.Concretes.Wallet;

namespace Wallet.Application.Managers
{
    public interface IWalletManager : IBaseManager<WalletEntity, WalletDto>
    {
        Task<decimal> GetBalanceAsync(string customerNo);
        Task<List<WalletDto>> GetWalletsByCustomerNoAsync(string customerNo);
        Task<WalletDto> CreateNewWalletAsync(string customerNo);
        Task<string> ProcessTransactionAsync(CreateTransactionDto transactionDto, string type);
    }
}
