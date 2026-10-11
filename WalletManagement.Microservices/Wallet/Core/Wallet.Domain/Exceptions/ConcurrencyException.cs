
namespace Wallet.Domain.Exceptions
{
    public class ConcurrencyException : BaseBusinessException
    {
        public const string code = "ERR_CONCURRENCY_CONFLICT";
        public ConcurrencyException() : base(code)
        {

        }
    }
}
