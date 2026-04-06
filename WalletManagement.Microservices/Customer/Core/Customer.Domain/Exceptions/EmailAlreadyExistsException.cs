namespace Customer.Domain.Exceptions
{
    public class EmailAlreadyExistsException : BaseBusinessException
    {
        public EmailAlreadyExistsException() : base("ERR_EMAIL_ADDRESS_ALREADY_REGISTERED")
        {

        }
    }
}
