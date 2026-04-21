namespace Investment.Domain.Exceptions
{
    public class TradeExecutionFailedOnWalletException : BaseBusinessException
    {
        public TradeExecutionFailedOnWalletException() : base("ERR_TRADE_EXECUTION_FAILED_ON_WALLET")
        {

        }
    }
}
