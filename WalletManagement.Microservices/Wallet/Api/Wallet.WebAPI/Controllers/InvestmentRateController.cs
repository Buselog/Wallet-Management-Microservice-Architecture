using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Application.Services;

namespace Wallet.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InvestmentRateController : ControllerBase
    {
        private readonly IInvestmentRateService _service;

        public InvestmentRateController(IInvestmentRateService service)
        {
            _service = service;
        }

        [HttpGet("rates")]
        public async Task<IActionResult> GetRates()
        {
            var result = await _service.GetDailyRatesAsync();
            return Ok(result);
        }
    }
}
