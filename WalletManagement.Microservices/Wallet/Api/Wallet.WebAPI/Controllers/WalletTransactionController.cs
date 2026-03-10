using Microsoft.AspNetCore.Mvc;
using Wallet.Application.Managers;

namespace Wallet.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletTransactionController : ControllerBase
    {
        private readonly ITransactionManager _transactionManager;

        public WalletTransactionController(ITransactionManager transactionManager)
        {
            _transactionManager = transactionManager;
        }

        [HttpGet("history/{walletId}")]
        public async Task<IActionResult> GetHistory(int walletId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var (items, totalCount) = await _transactionManager.GetHistoryAsync(
                walletId, startDate, endDate, pageNumber, pageSize);

            return Ok(new { Items = items, TotalCount = totalCount });
        }
    }
}

