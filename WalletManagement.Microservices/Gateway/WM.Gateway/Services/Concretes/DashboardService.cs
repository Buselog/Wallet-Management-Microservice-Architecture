using System.Net.Http.Headers;
using System.Text.RegularExpressions;
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

        public async Task<UserDashboardDto> GetUserSummaryAsync(string customerNo, string token, string culture)
        {
            var customerClient = _httpClientFactory.CreateClient("CustomerAPI");
            var walletClient = _httpClientFactory.CreateClient("WalletAPI");

            customerClient.DefaultRequestHeaders.AcceptLanguage.Clear();
            walletClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(culture));

            customerClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
            walletClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));

            var customerTask = customerClient.GetAsync($"api/Customer/getCustomerName-byCustomerNo/{customerNo}");
            var walletTask = walletClient.GetAsync("api/Wallet/wallets");

            await Task.WhenAll(customerTask, walletTask);

            string fullName = "Değerli Müşterimiz";
            if (customerTask.Result.IsSuccessStatusCode)
            {
                fullName = await customerTask.Result.Content.ReadAsStringAsync();
            }

            List<WalletDetailDto> wallets = new();
            if (walletTask.Result.IsSuccessStatusCode)
            {
                wallets = await walletTask.Result.Content.ReadFromJsonAsync<List<WalletDetailDto>>() ?? new();
            }

            var currencySummaries = wallets
               .GroupBy(w => w.Currency)
               .Select(group => new CurrencySummaryDto
               {
                   Currency = group.Key,
                   TotalBalance = group.Sum(x => x.Balance)
               }).ToList();

            return new UserDashboardDto
            {
                FullName = fullName,
                Wallets = wallets,
                CurrencySummaries = currencySummaries
            };
        }
    }
}
