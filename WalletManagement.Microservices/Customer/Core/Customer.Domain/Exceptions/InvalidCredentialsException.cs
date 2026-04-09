
namespace Customer.Domain.Exceptions
{
    public class InvalidCredentialsException : BaseBusinessException
    {
        public InvalidCredentialsException() : base("ERR_EMAIL_OR_PASSWORD_INCORRECT")
        {

        }
    }
}
