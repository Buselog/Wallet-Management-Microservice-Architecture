using WM.Gateway.Dtos;
using WM.Gateway.Services.Abstracts;

namespace WM.Gateway.Services.Concretes
{
    public class DashboardService : IDashboardService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DashboardService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<UserDashboardDto> GetUserSummaryAsync(string customerNo, string token)
        {
            var customerClient = _httpClientFactory.CreateClient("CustomerAPI");
            var walletClient = _httpClientFactory.CreateClient("WalletAPI");

            customerClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
            walletClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));

            var customerTask = customerClient.GetAsync($"api/Customer/getCustomerName-byCustomerNo/{customerNo}");
            var walletTask = walletClient.GetAsync("api/Wallet/wallets");

            await Task.WhenAll(customerTask, walletTask);

            var nameResponse = await customerTask.Result.Content.ReadAsStringAsync();
            var walletsResponse = await walletTask.Result.Content.ReadFromJsonAsync<List<WalletDetailDto>>();

            var currencySummaries = walletsResponse
               .GroupBy(w => w.Currency) 
               .Select(group => new CurrencySummaryDto
               {
                  Currency = group.Key, 
                  TotalBalance = group.Sum(x => x.Balance) 
               })
               .ToList();

            return new UserDashboardDto
            {
                FullName = nameResponse,
                Wallets = walletsResponse,
                CurrencySummaries = currencySummaries
            };
        }
    }
}
