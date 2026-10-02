namespace Wallet.Domain.Exceptions
{
    public class InvalidWalletTypeForTradeException : BaseBusinessException
    {

        public const string code = "ERR_INVALID_WALLET_TYPE_FOR_TRADE";
        public InvalidWalletTypeForTradeException() : base(code)
        {

        }
    }
}
