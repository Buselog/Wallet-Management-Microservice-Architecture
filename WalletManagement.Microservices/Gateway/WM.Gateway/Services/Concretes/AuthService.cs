using System.Net.Http.Headers;
using System.Text.RegularExpressions;
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

        public async Task<HttpResponseMessage> LoginAsync(UserLoginRequestDto loginRequestDto, string culture)
        {
            var client = _httpClientFactory.CreateClient("CustomerAPI");

            client.DefaultRequestHeaders.AcceptLanguage.Clear();
            client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(culture));
            return await client.PostAsJsonAsync("api/Auth/login", loginRequestDto);
        }

        public async Task<HttpResponseMessage> RegisterAsync(UserRegisterRequestDto registerRequestDto, string culture)
        {
            var client = _httpClientFactory.CreateClient("CustomerAPI");

            client.DefaultRequestHeaders.AcceptLanguage.Clear();
            client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(culture));

            return await client.PostAsJsonAsync("api/Auth/register", registerRequestDto);
        }
    }
}
