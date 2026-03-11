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

        private string currentCustomerNo => User.FindFirst("CustomerNo")?.Value!;

        [Authorize]
        [HttpGet("{id}/balance")]
        public async Task<IActionResult> GetBalance(int id)
        {
            var balance = await _walletManager.GetBalanceAsync(id, currentCustomerNo);
            return Ok(new { WalletId = id, Balance = balance });
        }

        [Authorize]
        [HttpGet("customer/{customerNo}")]
        public async Task<IActionResult> GetCustomerWallets()
        {
            var wallets = await _walletManager.GetWalletsByCustomerNoAsync(currentCustomerNo);
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
        public async Task<IActionResult> Deposit([FromBody] DepositRequestDto dto)
        {
            var message = await _walletManager.DepositAsync(dto, currentCustomerNo);
            return Ok(new { Message = message });
        }

        [Authorize]
        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawRequestDto dto)
        {
            var message = await _walletManager.WithdrawAsync(dto, currentCustomerNo);
            return Ok(new { Message = message });
        }

        [Authorize]
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferRequestDto dto)
        {
            var message = await _walletManager.TransferAsync(dto, currentCustomerNo);
            return Ok(new { Message = message });
        }
    }

}

