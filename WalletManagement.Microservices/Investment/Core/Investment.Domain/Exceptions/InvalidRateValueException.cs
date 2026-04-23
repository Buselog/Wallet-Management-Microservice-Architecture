namespace Investment.Domain.Exceptions
{
    public class InvalidRateValueException : BaseBusinessException
    {
        public InvalidRateValueException() : base("ERR_INVALID_RATE_VALUE")
        {

        }
    }
}
