
namespace Wallet.Domain.Exceptions
{
    public class InsufficientBalanceException : BaseBusinessException
    {
        public InsufficientBalanceException() : base("Cüzdan bakiyesi bu işlem için yetersiz.")
        {

        }
    }
}
