
using Document.Application.Dtos;
using Document.Application.Services;

namespace Document.InnerInfrastructure.Services
{
    public class WalletClient : IWalletClient
    {
        private readonly HttpClient _httpClient;

        public WalletClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> DeductBalanceAsync(WalletTransactionRequestDto request)
        {
            await Task.Delay(100);
            return true;
        }
    }
}
