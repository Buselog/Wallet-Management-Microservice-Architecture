
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

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/Wallet/create");
            httpRequest.Content = JsonContent.Create(request);

            httpRequest.Headers.Add("X-Service-Token", "WalletAppManagement_Internal_Secret_Key_2026");

            var response = await _httpClient.SendAsync(httpRequest);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();

                Log.Warning("Cüzdan servisi tarafında hata oluştu: {Status} - Detay: {Detail}",
                             response.StatusCode, errorContent);
            }
        }
    }
}
