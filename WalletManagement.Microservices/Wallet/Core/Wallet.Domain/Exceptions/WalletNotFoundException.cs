
namespace Wallet.Domain.Exceptions
{
    public class WalletNotFoundException : BaseBusinessException
    {
        public WalletNotFoundException() : base("ERR_WALLET_NOT_FOUND")
        {

        }
    }
}
