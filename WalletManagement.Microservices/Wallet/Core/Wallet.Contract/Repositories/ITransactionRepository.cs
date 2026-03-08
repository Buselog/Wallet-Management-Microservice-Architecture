using Wallet.Domain.Entities.Concretes;

namespace Wallet.Contract.Repositories
{
    public interface ITransactionRepository : IBaseRepository<WalletTransaction>
    {
        Task<(IEnumerable<WalletTransaction> Items, int TotalCount)> GetFilteredHistoryAsync(
            int walletId,
            DateTime? startDate,
            DateTime? endDate, 
            int pageNumber,
            int pageSize);

        Task<bool> ReferenceIdExistsAsync(string referenceId);
    }
}
