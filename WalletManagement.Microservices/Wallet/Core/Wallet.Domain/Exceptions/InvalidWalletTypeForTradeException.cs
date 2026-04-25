namespace Wallet.Domain.Exceptions
{
    public class InvalidWalletTypeForTradeException : BaseBusinessException
    {
        public InvalidWalletTypeForTradeException() : base("ERR_INVALID_WALLET_TYPE_FOR_TRADE")
        {

        }
    }
}
