
namespace Wallet.Domain.Exceptions
{
    public class InvalidIbanException : BaseBusinessException
    {
        public InvalidIbanException() : base("Geçersiz IBAN formatı.")
        {

        }
    }
}
