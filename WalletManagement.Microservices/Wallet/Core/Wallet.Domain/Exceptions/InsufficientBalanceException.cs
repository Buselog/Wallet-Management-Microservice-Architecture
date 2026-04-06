
namespace Wallet.Domain.Exceptions
{
    public class InsufficientBalanceException : BaseBusinessException
    {
        public InsufficientBalanceException() : base("ERR_LOW_BALANCE")
        {

        }
    }
}
