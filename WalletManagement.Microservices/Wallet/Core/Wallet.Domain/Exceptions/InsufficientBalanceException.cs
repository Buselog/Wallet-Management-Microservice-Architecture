
namespace Wallet.Domain.Exceptions
{
    public class InsufficientBalanceException : BaseBusinessException
    {
        public const string code = "ERR_LOW_BALANCE";
        public InsufficientBalanceException() : base(code)
        {

        }
    }
}
