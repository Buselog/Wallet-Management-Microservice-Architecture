namespace Investment.Application.Services
{
    public interface ITradeService
    {
        Task<bool> BuyCurrencyAsync(int sourceWalletId, int targetWalletId, 
            string customerNo, string currencyCode, decimal amount);

        Task<bool> SellCurrencyAsync(int sourceWalletId, int targetWalletId,
            string customerNo, string currencyCode, decimal amount);
    }
}
