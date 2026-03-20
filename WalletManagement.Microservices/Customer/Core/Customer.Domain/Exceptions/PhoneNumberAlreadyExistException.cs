namespace Customer.Domain.Exceptions
{
    public class PhoneNumberAlreadyExistException : BaseBusinessException
    {
        public PhoneNumberAlreadyExistException() : base("Bu telefon numarası zaten kayıtlı.")
        {

        }
    }
}
