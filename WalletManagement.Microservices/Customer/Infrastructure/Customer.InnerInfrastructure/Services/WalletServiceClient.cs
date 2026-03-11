
using Customer.Application.Managers;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Net.Http.Json;

namespace Customer.InnerInfrastructure.Services
{
    public class WalletServiceClient : IWalletServiceClient
    {
        private readonly HttpClient _httpClient;

        public WalletServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task CreateDefaultWalletAsync(string customerNo)
        {
            var request = new
            {
                CustomerNo = customerNo,
                Currency = "TRY",
                Type = 1 
            };

            var response = await _httpClient.PostAsJsonAsync("api/Wallet/create", request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();

                Log.Warning("Cüzdan servisi tarafında hata oluştu: {Reason} - Detay: {Detail}",
                             response.ReasonPhrase, errorContent);
            }
        }
    }
}
