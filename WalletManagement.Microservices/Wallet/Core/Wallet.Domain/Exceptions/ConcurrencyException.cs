
namespace Wallet.Domain.Exceptions
{
    public class ConcurrencyException : BaseBusinessException
    {
        public ConcurrencyException() : base("ERR_CONCURRENCY_CONFLICT")
        {

        }
    }
}
