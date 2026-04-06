using Investment.Application.Dtos;
using Investment.Application.Services;
using Investment.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Investment.InnerInfrastructure.Services
{
    public class InvestmentRateService : IInvestmentRateService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public InvestmentRateService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["InvestmentApi:Key"];
        }

        public async Task<List<ExchangeRateDto>> GetDailyRatesAsync()
        {
            var startDate = DateTime.Now.AddDays(-4).ToString("dd-MM-yyyy");
            var endDate = DateTime.Now.ToString("dd-MM-yyyy");

            var series = "TP.DK.USD.A.YTL-TP.DK.USD.S.YTL-TP.DK.EUR.A.YTL-TP.DK.EUR.S.YTL-TP.DK.GBP.A.YTL-TP.DK.GBP.S.YTL-TP.DK.CHF.A.YTL-TP.DK.CHF.S.YTL-TP.DK.JPY.A.YTL-TP.DK.JPY.S.YTL";

            var url = $"series={series}&startDate={startDate}&endDate={endDate}&type=json";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("key", _apiKey);

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new BaseBusinessException("ERR_FETCHING_EXCHANGE_RATES");

            var jsonContent = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(jsonContent);
            var items = doc.RootElement.GetProperty("items").EnumerateArray().ToList();

            var latestValidItem = items
                .Where(x => x.TryGetProperty("TP_DK_USD_A_YTL", out var val) && val.ValueKind != JsonValueKind.Null)
                .LastOrDefault();

            if (latestValidItem.ValueKind == JsonValueKind.Undefined) return new List<ExchangeRateDto>();

         
            return new List<ExchangeRateDto>
            {

                CreateDto("USD", latestValidItem, "TP_DK_USD_A_YTL", "TP_DK_USD_S_YTL"),
                CreateDto("EUR", latestValidItem, "TP_DK_EUR_A_YTL", "TP_DK_EUR_S_YTL"),
                CreateDto("GBP", latestValidItem, "TP_DK_GBP_A_YTL", "TP_DK_GBP_S_YTL"),
                CreateDto("CHF", latestValidItem, "TP_DK_CHF_A_YTL", "TP_DK_CHF_S_YTL"),
                CreateDto("JPY", latestValidItem, "TP_DK_JPY_A_YTL", "TP_DK_JPY_S_YTL")
            };

        }

        private ExchangeRateDto CreateDto(string code, JsonElement item, string buyProperty, string sellProperty)
        {
            return new ExchangeRateDto
            {
                CurrencyCode = code,
                BuyingRate = ParseDecimal(item, buyProperty),
                SellingRate = ParseDecimal(item, sellProperty),
                Date = DateTime.Now
            };
        }

        private decimal ParseDecimal(JsonElement item, string propertyName)
        {
            if (item.TryGetProperty(propertyName, out var val) && val.ValueKind != JsonValueKind.Null)
            {
                return decimal.Parse(val.GetString(), System.Globalization.CultureInfo.InvariantCulture);
            }
            return 0;
        }
    }
}
