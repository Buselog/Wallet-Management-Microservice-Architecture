using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Wallet.Contract.Repositories;
using Wallet.Persistence.Context;
using WalletEntity = Wallet.Domain.Entities.Concretes.Wallet;

namespace Wallet.Persistence.Repositories
{
    public class WalletRepository : BaseRepository<WalletEntity>, IWalletRepository
    {
        public WalletRepository(WalletContext context) : base(context) { 
        
        }

        public async Task<WalletEntity?> GetByCustomerNoAsync(string customerNo)
        {
           var wallet =  await _context.Wallets.FirstOrDefaultAsync(w => w.CustomerNo == customerNo);
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
            
        public async Task<bool> ExecuteMoneyTransactionWithSPAsync(int walletId, decimal amount, string type, string targetAddress, string referenceId)
        {
            var WalletId = new SqlParameter("@WalletId", walletId);
            var Amount = new SqlParameter("@Amount", amount);
            var Type = new SqlParameter("@Type", type);
            var TargetAddress = new SqlParameter("@TargetAddress", targetAddress);
            var ReferenceId = new SqlParameter("@ReferenceId", referenceId);

            var result = await _context.Database.ExecuteSqlRawAsync(
                "EXEC WalletTransactionSP @WalletId, @Amount, @Type, @TargetAddress, @ReferenceId",
                WalletId, Amount, Type, TargetAddress, ReferenceId);

            return result != 0;
        }
    }
}


