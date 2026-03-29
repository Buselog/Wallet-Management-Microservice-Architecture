using Investment.Application.Dtos;

namespace Investment.Application.Services
{
    public interface IInvestmentRateService
    {
        public Task<List<ExchangeRateDto>> GetDailyRatesAsync();
    }
}
