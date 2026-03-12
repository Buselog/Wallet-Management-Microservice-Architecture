using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Wallet.Contract.Repositories;
using Wallet.Persistence.Context;
using WalletEntity = Wallet.Domain.Entities.Concretes.Wallet;

namespace Wallet.Persistence.Repositories
{
    public class WalletRepository : BaseRepository<WalletEntity>, IWalletRepository
    {
        public WalletRepository(WalletContext context) : base(context)
        {

        }

        public async Task<WalletEntity?> GetByCustomerNoAsync(string customerNo)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.CustomerNo == customerNo);
            return wallet;
        }

        public async Task<WalletEntity?> GetByIbanAsync(string iban)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.IBAN == iban);
            return wallet;
        }

        public async Task<List<WalletEntity>> GetWalletsByCustomerNoAsync(string customerNo)
        {
            var wallets = await _context.Wallets.Where(w => w.CustomerNo == customerNo).ToListAsync();
            return wallets;
        }

        public async Task<bool> SoftDeleteWalletWithSPAsync(int walletId, string userCode)
        {
            var WalletId = new SqlParameter("@WalletId", walletId);
            var UserCode = new SqlParameter("@UserCode", userCode);

            var result = await _context.Database.ExecuteSqlRawAsync(
                "EXEC SoftDeleteWalletSP @WalletId, @UserCode", WalletId, UserCode);

            return result != 0;
        }

        public async Task<int> ExecuteMoneyTransactionWithSPAsync(int walletId, decimal amount, string type, string target, string referenceId)
        {
            var WalletId = new SqlParameter("@WalletId", walletId);
            var Amount = new SqlParameter("@Amount", amount);
            var Type = new SqlParameter("@Type", type);
            var Target = new SqlParameter("@Target", target);
            var ReferenceId = new SqlParameter("@ReferenceId", referenceId);

            var result = await _context.Database.ExecuteSqlRawAsync(
                "EXEC WalletTransactionSP @WalletId, @Amount, @Type, @Target, @ReferenceId",
                WalletId, Amount, Type, Target, ReferenceId);

            return result;
        }
    }
}


