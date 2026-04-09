
namespace Customer.Domain.Exceptions
{
    public class CustomerNotFoundException : BaseBusinessException
    {
        public CustomerNotFoundException() : base("ERR_CUSTOMER_NOT_FOUND_FOR_CUSTOMER_SERVICE")
        {

        }
    }
}
