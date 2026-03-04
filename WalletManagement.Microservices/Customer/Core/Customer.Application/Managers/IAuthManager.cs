using Customer.Application.Dtos;

namespace Customer.Application.Managers
{
    public interface IAuthManager
    {
        Task<string> RegisterAsync(CustomerRegisterDto registerDto);
        Task<LoginResponseDto> LoginAsync(CustomerLoginDto loginDto);
    }
}
