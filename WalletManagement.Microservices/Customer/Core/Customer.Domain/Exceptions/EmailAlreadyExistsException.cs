namespace Customer.Domain.Exceptions
{
    public class EmailAlreadyExistsException : BaseBusinessException
    {
        public EmailAlreadyExistsException() : base("Bu email adresi zaten kayıtlı.")
        {

        }
    }
}
