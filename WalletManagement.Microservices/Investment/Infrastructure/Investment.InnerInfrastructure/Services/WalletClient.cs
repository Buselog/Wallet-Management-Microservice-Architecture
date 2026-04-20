using Investment.Application.Dtos;
using Investment.Application.Services;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;

namespace Investment.InnerInfrastructure.Services
{
    public class WalletClient : IWalletClient
    {

        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WalletClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> ExecuteTradeAsync(CurrencyTradeRequestDto tradeRequest)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", token);
            }

            var response = await _httpClient.PostAsJsonAsync("api/Wallet/execute-trade", tradeRequest);

            return response.IsSuccessStatusCode;
        }
    }
}
