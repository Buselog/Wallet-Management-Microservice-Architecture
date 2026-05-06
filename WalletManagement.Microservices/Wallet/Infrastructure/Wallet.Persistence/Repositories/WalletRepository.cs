using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Wallet.Contract.Repositories;
using Wallet.Domain.Entities.Concretes;
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

        public async Task<List<WalletEntity>> GetWalletsByCustomerNoAsync(string customerNo, bool onlyActive = true)
        {

            var allWallets = _context.Wallets.Where(w => w.CustomerNo == customerNo);

            if (onlyActive)
            {
                allWallets = allWallets.Where(w => w.IsActive == true);
            }

            return await allWallets.ToListAsync();
        }

        public async Task<bool> AnyActiveWalletWithCurrencyAsync(string customerNo, string currency)
        {
            return await _context.Wallets.AnyAsync(w => w.CustomerNo == customerNo && w.Currency == currency && w.IsActive);
        }

        public async Task<bool> SoftDeleteWalletWithSPAsync(int walletId, string userCode)
        {
            var WalletId = new SqlParameter("@WalletId", walletId);
            var UserCode = new SqlParameter("@UserCode", userCode);

            var result = await _context.Database.ExecuteSqlRawAsync(
                "EXEC SoftDeleteWalletSP @WalletId, @UserCode", WalletId, UserCode);

            return result != 0;
        }

        public async Task<int> ExecuteMoneyTransactionWithSPAsync(int walletId, decimal amount, string type, string target, string description ,string referenceId, string senderInfo)
        {
            var WalletId = new SqlParameter("@WalletId", walletId);
            var Amount = new SqlParameter("@Amount", amount);
            var Type = new SqlParameter("@Type", type);
            var Target = new SqlParameter("@Target", target ?? (object)DBNull.Value);
            var Description = new SqlParameter("@Description", description ?? (object)DBNull.Value);
            var ReferenceId = new SqlParameter("@ReferenceId", referenceId);
            var SenderInfo = new SqlParameter("@SenderInfo", senderInfo ?? (object)DBNull.Value);

            var result = await _context.Database
                .SqlQueryRaw<int>(
                    "EXEC WalletTransactionSP @WalletId, @Amount, @Type, @Target, @Description, @ReferenceId, @SenderInfo",
                    WalletId, Amount, Type, Target, Description, ReferenceId, SenderInfo)
                .ToListAsync();

            return result.FirstOrDefault();
        }

        public async Task<int> ExecuteCurrencyTradeWithSPAsync(string customerNo, int sourceWalletId, int targetWalletId, decimal amount, decimal targetRate, string tradeType, string referenceId)
        {
            var CustomerNo = new SqlParameter("@CustomerNo", customerNo);
            var SourceId = new SqlParameter("@SourceWalletId", sourceWalletId);
            var TargetId = new SqlParameter("@TargetWalletId", targetWalletId);
            var Amount = new SqlParameter("@Amount", amount);
            var Rate = new SqlParameter("@TargetRate", targetRate);
            var TradeType = new SqlParameter("@TradeType", tradeType);
            var ReferenceId = new SqlParameter("@ReferenceId", referenceId);

            var result = await _context.Database
                .SqlQueryRaw<int>(
                    "EXEC SP_ExecuteCurrencyTrade @CustomerNo, @SourceWalletId, @TargetWalletId, @Amount, @TargetRate, @TradeType, @ReferenceId",
                    CustomerNo, SourceId, TargetId, Amount, Rate, TradeType, ReferenceId)
                .ToListAsync();

            return result.FirstOrDefault();
        }
    }
}


