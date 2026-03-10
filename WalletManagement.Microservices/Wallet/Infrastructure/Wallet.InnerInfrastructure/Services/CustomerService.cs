
namespace Wallet.InnerInfrastructure.Services
{
    public class CustomerService
    {
        private readonly HttpClient _httpClient;

        public CustomerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> GetCustomerNoByPhoneAsync(string phoneNumber)
        {
            var response = await _httpClient.GetAsync($"api/customers/getCustomerNo-byPhone/{phoneNumber}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return null; 
        }
    }
}

