using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Application.Managers;
using Wallet.Contract.Repositories;

namespace Wallet.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WalletTransactionController : ControllerBase
    {
        private readonly ITransactionManager _transactionManager;

        private string currentCustomerNo => User.FindFirst("CustomerNo")?.Value!;

        public WalletTransactionController(ITransactionManager transactionManager)
        {
            _transactionManager = transactionManager;
        }

        [HttpGet("history/{walletId}")]
        public async Task<IActionResult> GetHistory(int walletId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var (items, totalCount) = await _transactionManager.GetHistoryAsync(
                walletId, currentCustomerNo, startDate, endDate, pageNumber, pageSize);

            return Ok(new { Items = items, TotalCount = totalCount });
        }
    }
}

