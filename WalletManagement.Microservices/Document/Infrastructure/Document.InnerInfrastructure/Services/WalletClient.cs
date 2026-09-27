using Document.Application.Dtos;
using Document.Application.Exceptions;
using Document.Application.Services;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace Document.InnerInfrastructure.Services
{
    public class WalletClient : IWalletClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        public WalletClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        private void AttachBearerToken()
        {
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", token);
            }
        }

        public async Task<List<WalletDto>> GetUserWalletsAsync()
        {
            AttachBearerToken();

            var response = await _httpClient.GetAsync("api/Wallet/wallets");

            if (!response.IsSuccessStatusCode)
            {
                await HandleErrorAsync(response, "ERR_FAILED_TO_FETCH_WALLETS");
            }

            var wallets = await response.Content.ReadFromJsonAsync<List<WalletDto>>(JsonOptions);
            return wallets ?? new List<WalletDto>();
        }

        public async Task<bool> PayInvoiceAsync(PayInvoiceRequestDto payRequest)
        {
            AttachBearerToken();

            var response = await _httpClient.PostAsJsonAsync("api/Wallet/withdraw", payRequest);

            if (!response.IsSuccessStatusCode)
            {
                await HandleErrorAsync(response, "ERR_INVOICE_PAYMENT_FAILED_ON_WALLET");
            }

            return true;
        }

        private static async Task HandleErrorAsync(HttpResponseMessage response, string defaultErrorCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();

            try
            {
                var errorObj = JsonSerializer.Deserialize<JsonElement>(errorContent);

                string errorMessage = defaultErrorCode;
                if (errorObj.TryGetProperty("Message", out var msgProp) || errorObj.TryGetProperty("message", out msgProp))
                {
                    errorMessage = msgProp.GetString() ?? defaultErrorCode;
                }

                object[]? parameters = null;
                if ((errorObj.TryGetProperty("Parameters", out var paramElement) || errorObj.TryGetProperty("parameters", out paramElement))
                    && paramElement.ValueKind == JsonValueKind.Array)
                {
                    parameters = JsonSerializer.Deserialize<object[]>(paramElement.GetRawText());
                }

                throw new WalletServiceException((int)response.StatusCode, errorMessage, parameters);
            }
            catch (WalletServiceException)
            {
                throw;
            }
            catch
            {
                throw new WalletServiceException((int)response.StatusCode, defaultErrorCode, new object[] { errorContent });
            }
        }
    }
}