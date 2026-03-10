
namespace Customer.Domain.Exceptions
{
    public class InvalidCredentialsException : BaseBusinessException
    {
        public InvalidCredentialsException() : base("Email veya şifre hatalı.")
        {

        }
    }
}
