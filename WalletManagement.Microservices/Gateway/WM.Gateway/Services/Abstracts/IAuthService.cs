using WM.Gateway.Dtos;

namespace WM.Gateway.Services.Abstracts
{
    public interface IAuthService
    {
        Task<HttpResponseMessage> LoginAsync(UserLoginRequestDto loginRequestDto);
        Task<HttpResponseMessage> RegisterAsync(UserRegisterRequestDto registerRequestDto);

    }
}
