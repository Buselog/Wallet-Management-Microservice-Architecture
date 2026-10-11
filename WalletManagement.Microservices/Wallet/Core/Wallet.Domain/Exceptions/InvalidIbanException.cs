
namespace Wallet.Domain.Exceptions
{
    public class InvalidIbanException : BaseBusinessException
    {
        public const string code = "ERR_INVALID_IBAN";
        public InvalidIbanException() : base(code)
        {

        }
    }
}
