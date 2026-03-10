
namespace Customer.Domain.Exceptions
{
    public class CustomerNotFoundException : BaseBusinessException
    {
        public CustomerNotFoundException() : base("Müşteri bulunamadı.")
        {

        }
    }
}
