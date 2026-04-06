
namespace Wallet.Domain.Exceptions
{
    public class InvalidIbanException : BaseBusinessException
    {
        public InvalidIbanException() : base("ERR_INVALID_IBAN")
        {

        }
    }
}
