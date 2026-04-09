namespace Customer.Domain.Exceptions
{
    public class PhoneNumberAlreadyExistException : BaseBusinessException
    {
        public PhoneNumberAlreadyExistException() : base("ERR_PHONE_NUMBER_ALREADY_REGISTERED")
        {

        }
    }
}
