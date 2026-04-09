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
        [HttpGet("wallets/")]
        public async Task<IActionResult> GetCustomerWallets()
        {
            var wallets = await _walletManager.GetWalletsByCustomerNoAsync(currentCustomerNo);
            return Ok(wallets);
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateWallet([FromBody] CreateWalletRequestDto request)
        {
            var result = await _walletManager.CreateNewWalletAsync(currentCustomerNo, request.Currency, request.Type);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositRequestDto dto)
        {
            await _walletManager.DepositAsync(dto, currentCustomerNo);
            return Ok();
        }

        [Authorize]
        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawRequestDto dto)
        {
            await _walletManager.WithdrawAsync(dto, currentCustomerNo);
            return Ok();
        }

        [Authorize]
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferRequestDto dto)
        {
            await _walletManager.TransferAsync(dto, currentCustomerNo);
            return Ok();
        }


        [Authorize]
        [HttpDelete("{id}/delete")]
        public async Task<IActionResult> DeleteWallet(int id)
        {
            await _walletManager.SoftDeleteWalletAsync(id, currentCustomerNo);
            return Ok();
        }
    }

}

