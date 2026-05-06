namespace Wallet.Domain.Exceptions
{
    public class WalletCurrencyMismatchException : BaseBusinessException
    {

        public WalletCurrencyMismatchException(string message) : base(message)
        {

        }

        public WalletCurrencyMismatchException(string errorCode, params object[] parameters) : base(errorCode, parameters)
        {
           
        }
    }
}
