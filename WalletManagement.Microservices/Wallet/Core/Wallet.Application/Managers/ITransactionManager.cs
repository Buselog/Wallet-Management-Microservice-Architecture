using Wallet.Application.Dtos;
using Wallet.Domain.Entities.Concretes;

namespace Wallet.Application.Managers
{
    public interface ITransactionManager : IBaseManager<WalletTransaction, WalletTransactionDto>
    {
        Task<(List<WalletTransactionDto> Items, int TotalCount)> GetHistoryAsync(
            int walletId,
            DateTime? startDate,
            DateTime? endDate,
            int pageNumber,
            int pageSize);
    }
}
