namespace Investment.Domain.Exceptions
{
    public class CurrencyRateNotFoundException : BaseBusinessException
    {
        public CurrencyRateNotFoundException() : base("ERR_CURRENCY_RATE_NOT_FOUND")
        {

        }
    }
}
