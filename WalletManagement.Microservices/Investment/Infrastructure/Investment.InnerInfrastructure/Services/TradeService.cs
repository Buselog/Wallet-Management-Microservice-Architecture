using Investment.Application.Dtos;
using Investment.Application.Services;
using Investment.Domain.Exceptions;

namespace Investment.InnerInfrastructure.Services
{
    public class TradeService : ITradeService
    {
        private readonly IInvestmentRateService _rateService;
        private readonly IWalletClient _walletClient;

        public TradeService(IInvestmentRateService rateService, IWalletClient walletClient)
        {
            _rateService = rateService;
            _walletClient = walletClient;
        }

        public async Task<bool> BuyCurrencyAsync(int sourceWalletId, int targetWalletId, string customerNo, string currencyCode, decimal amount)
        {
            var allRates = await _rateService.GetDailyRatesAsync();

            var targetRate = allRates.FirstOrDefault(x => x.CurrencyCode == currencyCode);

            if (targetRate == null)
                throw new BaseBusinessException("ERR_CURRENCY_RATE_NOT_FOUND");

            if (targetRate.SellingRate <= 0)
                throw new BaseBusinessException("ERR_INVALID_RATE_VALUE");

            var tradeRequest = new CurrencyTradeRequestDto
            {
                CustomerNo = customerNo,
                SourceWalletId = sourceWalletId,
                TargetWalletId = targetWalletId,
                Amount = amount, 
                TargetRate = targetRate.SellingRate, 
                TradeType = "BUY"
            };

            var result = await _walletClient.ExecuteTradeAsync(tradeRequest);

            if (!result)
                throw new BaseBusinessException("ERR_TRADE_EXECUTION_FAILED_ON_WALLET");

            return result;
        }

        public async Task<bool> SellCurrencyAsync(int sourceWalletId, int targetWalletId, string customerNo, string currencyCode, decimal amount)
        {
            var allRates = await _rateService.GetDailyRatesAsync();

            var targetRate = allRates.FirstOrDefault(x => x.CurrencyCode == currencyCode);

            if (targetRate == null) throw new BaseBusinessException("ERR_CURRENCY_RATE_NOT_FOUND");

            var tradeRequest = new CurrencyTradeRequestDto
            {
                CustomerNo = customerNo,
                SourceWalletId = sourceWalletId, 
                TargetWalletId = targetWalletId, 
                Amount = amount,
                TargetRate = targetRate.BuyingRate,
                TradeType = "SELL"
            };

            var result = await _walletClient.ExecuteTradeAsync(tradeRequest);

            if (!result)
                throw new BaseBusinessException("ERR_TRADE_EXECUTION_FAILED_ON_WALLET");

            return result;
        }
      
    }
}
