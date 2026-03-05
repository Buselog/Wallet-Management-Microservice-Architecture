using Customer.Application.Dtos;
using Customer.Application.Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Customer.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthManager _authManager;

        public AuthController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CustomerRegisterDto registerDto)
        {
            var result = await _authManager.RegisterAsync(registerDto);

            return Ok(new { Message = result });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] CustomerLoginDto loginDto)
        {
            var result = await _authManager.LoginAsync(loginDto);

            return Ok(result);
        }
    }
}
