using Investment.Application.Dtos;

namespace Investment.Application.Services
{
    public interface IWalletClient
    {
        Task<bool> ExecuteTradeAsync(CurrencyTradeRequestDto tradeRequest);
    }
}
