using WalletEntity = Wallet.Domain.Entities.Concretes.Wallet;

namespace Wallet.Contract.Repositories
{
    public interface IWalletRepository : IBaseRepository<WalletEntity>
    {
        Task<WalletEntity?> GetByCustomerNoAsync(string customerNo);
        Task<WalletEntity?> GetByIbanAsync(string iban);
        Task<List<WalletEntity>> GetWalletsByCustomerNoAsync(string customerNo, bool onlyActive = true);
        Task<bool> AnyActiveWalletWithCurrencyAsync(string customerNo, string currency);
        Task<bool> SoftDeleteWalletWithSPAsync(int walletId, string userCode);
        Task<int> ExecuteMoneyTransactionWithSPAsync(int walletId, decimal amount, 
            string type, string targetAddress, string description, string referenceId, string senderInfo);
        Task<int> ExecuteCurrencyTradeWithSPAsync(string customerNo, int sourceWalletId, 
            int targetWalletId, decimal amount, decimal targetRate, string tradeType, string referenceId);

    }
}

