namespace Wallet.Domain.Exceptions
{
    public class WalletBalanceIsNotEmptyExcepiton : BaseBusinessException
    {
        public const string code = "ERR_WALLET_BALANCE_NOT_EMPTY";
        public WalletBalanceIsNotEmptyExcepiton() 
            : base(code)
        {

        }
    }
}
