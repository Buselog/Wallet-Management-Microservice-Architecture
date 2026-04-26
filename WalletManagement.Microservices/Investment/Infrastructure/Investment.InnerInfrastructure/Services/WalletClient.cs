using Investment.Application.Dtos;
using Investment.Application.Services;
using Investment.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;

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

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();

                var errorObj = JsonSerializer.Deserialize<JsonElement>(errorContent);

                string errorMessage = errorObj.GetProperty("Message").GetString() ?? "ERR_TRADE_EXECUTION_FAILED_ON_WALLET";

                object[] parameters = null;
                if (errorObj.TryGetProperty("Parameters", out var paramElement) && paramElement.ValueKind == JsonValueKind.Array)
                {
                    parameters = JsonSerializer.Deserialize<object[]>(paramElement.GetRawText());
                }

                throw new WalletServiceException((int)response.StatusCode, errorMessage, parameters);
            }

            return true;
        }
    }
}
