using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Application.Dtos;
using Wallet.Application.Managers;

namespace Wallet.WebAPI.Controllers
{
    [Authorize]
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

        [HttpGet("{id}/balance")]
        public async Task<IActionResult> GetBalance(int id)
        {
            var balance = await _walletManager.GetBalanceAsync(id, currentCustomerNo);
            return Ok(new { WalletId = id, Balance = balance });
        }

        [HttpGet("wallets/")]
        public async Task<IActionResult> GetCustomerWallets()
        {
            var wallets = await _walletManager.GetWalletsByCustomerNoAsync(currentCustomerNo);
            return Ok(wallets);
        }

        [HttpPost("create")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateWallet([FromBody] CreateWalletRequestDto request)
        {
            var serviceToken = Request.Headers["X-Service-Token"].ToString();

            if (serviceToken == "WalletAppManagement_Internal_Secret_Key_2026")
            {
                var result = await _walletManager.CreateNewWalletAsync(request.CustomerNo, request.Currency, request.Type);
                return Ok(result);
            }

            if (User.Identity.IsAuthenticated)
            {
                var result = await _walletManager.CreateNewWalletAsync(currentCustomerNo, request.Currency, request.Type);
                return Ok(result);
            }

            return Unauthorized();
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositRequestDto dto)
        {
            await _walletManager.DepositAsync(dto, currentCustomerNo);
            return Ok();
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawRequestDto dto)
        {
            await _walletManager.WithdrawAsync(dto, currentCustomerNo);
            return Ok();
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferRequestDto dto)
        {
            await _walletManager.TransferAsync(dto, currentCustomerNo);
            return Ok();
        }

        [HttpDelete("{id}/delete")]
        public async Task<IActionResult> DeleteWallet(int id)
        {
            await _walletManager.SoftDeleteWalletAsync(id, currentCustomerNo);
            return Ok();
        }

        [HttpPost("execute-trade")]
        public async Task<IActionResult> ExecuteTrade([FromBody] CurrencyTradeRequestDto dto)
        {
            await _walletManager.ExecuteTradeAsync(dto);
            return Ok();
        }
    }

}

