using WM.Gateway.Dtos;
using WM.Gateway.Services.Abstracts;

namespace WM.Gateway.Services.Concretes
{
    public class AuthService : IAuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<AuthResponseDto> LoginAsync(UserLoginRequestDto loginRequestDto)
        {
            var client = _httpClientFactory.CreateClient("CustomerAPI");
            var response = await client.PostAsJsonAsync("api/Auth/login", loginRequestDto);

            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        }

        public async Task<bool> RegisterAsync(UserRegisterRequestDto registerRequestDto)
        {
            var client = _httpClientFactory.CreateClient("CustomerAPI");
            var response = await client.PostAsJsonAsync("api/Auth/register", registerRequestDto);
            return response.IsSuccessStatusCode;

        }
    }
}
