namespace Wallet.Domain.Exceptions
{
    public class WalletCurrencyMismatchException : BaseBusinessException
    {

        public WalletCurrencyMismatchException() : base("ERR_WALLET_CURRENCY_MISMATCH")
        {

        }

        public WalletCurrencyMismatchException(string errorCode, params object[] parameters) : base(errorCode, parameters)
        {
           
        }
    }
}
