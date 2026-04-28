using Microsoft.AspNetCore.Mvc;
using WM.Gateway.Dtos;
using WM.Gateway.Services.Abstracts;

namespace WM.Gateway.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService) => _authService = authService;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            return result != null ? Ok(result) : Unauthorized();
        }
    }
}