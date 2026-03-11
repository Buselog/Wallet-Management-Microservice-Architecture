
namespace Wallet.InnerInfrastructure.Services
{
    public class CustomerServiceClient
    {
        private readonly HttpClient _httpClient;

        public CustomerServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> GetCustomerNoByPhoneAsync(string phoneNumber)
        {
            var response = await _httpClient.GetAsync($"api/Customer/getCustomerNo-byPhone/{phoneNumber}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return null; 
        }
    }
}

