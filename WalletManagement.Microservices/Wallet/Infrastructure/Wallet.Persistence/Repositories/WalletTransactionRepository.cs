using Microsoft.EntityFrameworkCore;
using Wallet.Contract.Repositories;
using Wallet.Domain.Entities.Concretes;
using Wallet.Persistence.Context;

namespace Wallet.Persistence.Repositories
{
    public class WalletTransactionRepository : BaseRepository<WalletTransaction>, ITransactionRepository
    {
        public WalletTransactionRepository(WalletContext context) : base(context) { 
        
        }

        public async Task<(IEnumerable<WalletTransaction> Items, int TotalCount)> GetFilteredHistoryAsync(
             int walletId, DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize)
        {
            var query = _context.WalletTransactions
                                .Where(t => t.WalletId == walletId)
                                .AsNoTracking();

            if (startDate.HasValue)
                query = query.Where(t => t.CreatedDate >= startDate.Value.Date);

            if (endDate.HasValue)
            {
                var endOfSelectedDay = endDate.Value.Date.AddDays(1).AddTicks(-1);

                query = query.Where(t => t.CreatedDate <= endOfSelectedDay);
            }

            int totalCount = await query.CountAsync();

            var items = await query.OrderByDescending(t => t.CreatedDate)
                                   .Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return (items, totalCount);
        }

        public async Task<bool> ReferenceIdExistsAsync(string referenceId)
        {
            return await _context.WalletTransactions.AnyAsync(t => t.ReferenceId == referenceId);
        }
    }
}
