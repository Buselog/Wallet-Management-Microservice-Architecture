using Wallet.Application.Dtos;

namespace Wallet.Application.Services
{
    public interface IInvestmentRateService 
    {
        public Task<List<ExchangeRateDto>> GetDailyRatesAsync();
    }
}


