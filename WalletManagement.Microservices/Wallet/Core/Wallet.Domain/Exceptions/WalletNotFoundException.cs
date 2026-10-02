
namespace Wallet.Domain.Exceptions
{
    public class WalletNotFoundException : BaseBusinessException
    {
        public const string code = "ERR_WALLET_NOT_FOUND";
        public WalletNotFoundException() : base(code)
        {

        }
    }
}
