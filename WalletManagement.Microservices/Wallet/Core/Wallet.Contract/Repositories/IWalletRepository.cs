using WalletEntity = Wallet.Domain.Entities.Concretes.Wallet;

namespace Wallet.Contract.Repositories
{
    public interface IWalletRepository : IBaseRepository<WalletEntity>
    {
        Task<WalletEntity?> GetByCustomerNoAsync(string customerNo);
        Task<WalletEntity?> GetByIbanAsync(string iban);
        Task<List<WalletEntity>> GetWalletsByCustomerNoAsync(string customerNo);
        Task<bool> SoftDeleteWalletWithSPAsync(int walletId, string userCode);
        Task<int> ExecuteMoneyTransactionWithSPAsync(int walletId, decimal amount, 
            string type, string targetAddress, string description, string referenceId);
    }
}

