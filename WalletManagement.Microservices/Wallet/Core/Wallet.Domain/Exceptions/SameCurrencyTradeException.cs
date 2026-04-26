namespace Wallet.Domain.Exceptions
{
    public class SameCurrencyTradeException : BaseBusinessException
    {
        public SameCurrencyTradeException() : base("ERR_SAME_CURRENCY_TRADE_NOT_ALLOWED")
        {

        }
    }
}
