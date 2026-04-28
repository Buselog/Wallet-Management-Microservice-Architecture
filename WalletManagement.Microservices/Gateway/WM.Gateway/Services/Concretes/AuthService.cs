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

        public async Task<HttpResponseMessage> LoginAsync(UserLoginRequestDto loginRequestDto)
        {
            var client = _httpClientFactory.CreateClient("CustomerAPI");
            return await client.PostAsJsonAsync("api/Auth/login", loginRequestDto);
        }

        public async Task<HttpResponseMessage> RegisterAsync(UserRegisterRequestDto registerRequestDto)
        {
            var client = _httpClientFactory.CreateClient("CustomerAPI");
            return await client.PostAsJsonAsync("api/Auth/register", registerRequestDto);

        }
    }
}
