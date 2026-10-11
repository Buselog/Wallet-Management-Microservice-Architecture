
namespace Wallet.Domain.Exceptions
{
    public class CustomerNotFoundException : BaseBusinessException
    {
        public const string code = "ERR_CUSTOMER_NOT_FOUND";
        public CustomerNotFoundException() : base(code)
        {

        }
    }
}
