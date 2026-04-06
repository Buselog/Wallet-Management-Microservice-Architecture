namespace Wallet.Domain.Exceptions
{
    public class WalletBalanceIsNotEmptyExcepiton : BaseBusinessException
    {

        public WalletBalanceIsNotEmptyExcepiton() 
            : base("ERR_WALLET_BALANCE_NOT_EMPTY")
        {

        }
    }
}
