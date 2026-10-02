namespace Wallet.Domain.Exceptions
{
    public class SameCurrencyTradeException : BaseBusinessException
    {
        public const string code = "ERR_SAME_CURRENCY_TRADE_NOT_ALLOWED";
        public SameCurrencyTradeException() : base(code)
        {

        }
    }
}
