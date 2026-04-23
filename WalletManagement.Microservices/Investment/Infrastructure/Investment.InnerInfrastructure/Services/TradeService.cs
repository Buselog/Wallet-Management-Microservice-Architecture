using Investment.Application.Dtos;
using Investment.Application.Services;
using Investment.Domain.Exceptions;

namespace Investment.InnerInfrastructure.Services
{
    public class TradeService : ITradeService
    {
        private readonly IInvestmentRateService _rateService;
        private readonly IWalletClient _walletClient;
        private readonly IReferenceGenerator _referenceGenerator;

        public TradeService(IInvestmentRateService rateService, IWalletClient walletClient, IReferenceGenerator referenceGenerator)
        {
            _rateService = rateService;
            _walletClient = walletClient;
            _referenceGenerator = referenceGenerator;
        }

        public async Task<bool> BuyCurrencyAsync(int sourceWalletId, int targetWalletId, string customerNo, string currencyCode, decimal amount)
        {
            var allRates = await _rateService.GetDailyRatesAsync();

            var targetRate = allRates.FirstOrDefault(x => x.CurrencyCode == currencyCode);

            if (targetRate == null)
                throw new CurrencyRateNotFoundException();

            if (targetRate.SellingRate <= 0)
                throw new InvalidRateValueException();

            var tradeRequest = new CurrencyTradeRequestDto
            {
                CustomerNo = customerNo,
                SourceWalletId = sourceWalletId,
                TargetWalletId = targetWalletId,
                Amount = amount,
                TargetRate = targetRate.SellingRate,
                TradeType = "BUY",
                ReferenceId = _referenceGenerator.GenerateTradeReference()
            };

            var result = await _walletClient.ExecuteTradeAsync(tradeRequest);

            return result;
        }

        public async Task<bool> SellCurrencyAsync(int sourceWalletId, int targetWalletId, string customerNo, string currencyCode, decimal amount)
        {
            var allRates = await _rateService.GetDailyRatesAsync();

            var targetRate = allRates.FirstOrDefault(x => x.CurrencyCode == currencyCode);

            if (targetRate == null) throw new CurrencyRateNotFoundException();

            var tradeRequest = new CurrencyTradeRequestDto
            {
                CustomerNo = customerNo,
                SourceWalletId = sourceWalletId, 
                TargetWalletId = targetWalletId, 
                Amount = amount,
                TargetRate = targetRate.BuyingRate,
                TradeType = "SELL",
                ReferenceId = _referenceGenerator.GenerateTradeReference()
            };

            var result = await _walletClient.ExecuteTradeAsync(tradeRequest);

            return result;
        }
      
    }
}
