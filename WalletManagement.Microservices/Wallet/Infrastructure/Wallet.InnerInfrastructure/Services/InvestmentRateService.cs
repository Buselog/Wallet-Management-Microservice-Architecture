using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Wallet.Application.Dtos;
using Wallet.Application.Services;
using Wallet.Domain.Exceptions;

namespace Wallet.InnerInfrastructure.Services
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
            var today = DateTime.Now.ToString("dd-MM-yyyy");

            var series = "TP.DK.USD.A.YTL-TP.DK.USD.S.YTL-TP.DK.EUR.A.YTL-TP.DK.EUR.S.YTL-TP.DK.GBP.A.YTL-TP.DK.GBP.S.YTL-TP.DK.CHF.A.YTL-TP.DK.CHF.S.YTL-TP.DK.JPY.A.YTL-TP.DK.JPY.S.YTL";

            var url = $"series={series}&startDate={today}&endDate={today}&type=json";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("key", _apiKey);

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new BaseBusinessException($"Kur bilgilerini çekerken bir hata oluştu: {response.StatusCode}");

            var jsonContent = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(jsonContent);
            var items = doc.RootElement.GetProperty("items");

            if (items.GetArrayLength() == 0) return new List<ExchangeRateDto>();

            var firstItem = items[0];

            return new List<ExchangeRateDto>
            {

                CreateDto("USD", firstItem, "TP_DK_USD_A_YTL", "TP_DK_USD_S_YTL"),
                CreateDto("EUR", firstItem, "TP_DK_EUR_A_YTL", "TP_DK_EUR_S_YTL"),
                CreateDto("GBP", firstItem, "TP_DK_GBP_A_YTL", "TP_DK_GBP_S_YTL"),
                CreateDto("CHF", firstItem, "TP_DK_CHF_A_YTL", "TP_DK_CHF_S_YTL"),
                CreateDto("JPY", firstItem, "TP_DK_JPY_A_YTL", "TP_DK_JPY_S_YTL")
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
