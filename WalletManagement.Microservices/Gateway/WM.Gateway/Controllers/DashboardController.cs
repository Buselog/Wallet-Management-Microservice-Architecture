using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WM.Gateway.Services.Abstracts;

namespace WM.Gateway.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _service;
        public DashboardController(IDashboardService service)
        {
            _service = service;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var customerNo = User.FindFirst("CustomerNo")?.Value;
            var token = Request.Headers["Authorization"].ToString();

            var result = await _service.GetUserSummaryAsync(customerNo!, token);
            return Ok(result);
        }
    }
}
