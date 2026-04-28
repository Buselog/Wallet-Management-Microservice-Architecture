using WM.Gateway.Dtos;

namespace WM.Gateway.Services.Abstracts
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(UserLoginRequestDto loginRequestDto);
        Task<bool> RegisterAsync(UserRegisterRequestDto registerRequestDto);

    }
}
