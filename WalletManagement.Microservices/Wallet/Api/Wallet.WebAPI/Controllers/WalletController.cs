using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Application.Dtos;
using Wallet.Application.Managers;

namespace Wallet.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletManager _walletManager;

        public WalletController(IWalletManager walletManager)
        {
            _walletManager = walletManager;
        }

        [Authorize]
        [HttpGet("{id}/balance")]
        public async Task<IActionResult> GetBalance(int id, [FromQuery] string customerNo)
        {
            var balance = await _walletManager.GetBalanceAsync(id, customerNo);
            return Ok(new { WalletId = id, Balance = balance });
        }

        [Authorize]
        [HttpGet("customer/{customerNo}")]
        public async Task<IActionResult> GetCustomerWallets(string customerNo)
        {
            var wallets = await _walletManager.GetWalletsByCustomerNoAsync(customerNo);
            return Ok(wallets);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateWallet([FromBody] CreateWalletRequestDto request)
        {
            var result = await _walletManager.CreateNewWalletAsync(request.CustomerNo, request.Currency, request.Type);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] CreateTransactionDto transactionDto)
        {
            var message = await _walletManager.ProcessTransactionAsync(transactionDto, "Deposit");
            return Ok(new { Message = message });
        }

        [Authorize]
        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] CreateTransactionDto transactionDto)
        {
            var message = await _walletManager.ProcessTransactionAsync(transactionDto, "Withdraw");
            return Ok(new { Message = message });
        }

        [Authorize]
        [HttpPost("transfer")]
        public async Task<IActionResult> TransferMoney([FromBody] CreateTransactionDto transactionDto)
        {
            var message = await _walletManager.ProcessTransactionAsync(transactionDto, "Transfer");
            return Ok(new { Message = message });
        }
    }

}

