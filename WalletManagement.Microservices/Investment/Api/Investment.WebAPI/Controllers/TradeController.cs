using Investment.Application.Dtos;
using Investment.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Investment.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradeController : ControllerBase
    {
        private readonly ITradeService _tradeService;

        public TradeController(ITradeService tradeService)
        {
            _tradeService = tradeService;
        }

        [HttpPost("buy")]
        public async Task<IActionResult> Buy([FromBody] TradeOperationRequest request)
        {
            var customerNo = User.Claims.FirstOrDefault(c => c.Type == "CustomerNo")?.Value;

            var result = await _tradeService.BuyCurrencyAsync(
                request.SourceWalletId,
                request.TargetWalletId,
                customerNo!,
                request.CurrencyCode,
                request.Amount);

            return Ok();
        }

        [HttpPost("sell")]
        public async Task<IActionResult> Sell([FromBody] TradeOperationRequest request)
        {
            var customerNo = User.Claims.FirstOrDefault(c => c.Type == "CustomerNo")?.Value;

            var result = await _tradeService.SellCurrencyAsync(
                request.SourceWalletId,
                request.TargetWalletId,
                customerNo!,
                request.CurrencyCode,
                request.Amount);

            return Ok();
        }
    }
}
