using WM.Gateway.Dtos;

namespace WM.Gateway.Services.Abstracts
{
    public interface IAuthService
    {
        Task<HttpResponseMessage> LoginAsync(UserLoginRequestDto loginRequestDto, string culture);
        Task<HttpResponseMessage> RegisterAsync(UserRegisterRequestDto registerRequestDto, string culture);

    }
}
