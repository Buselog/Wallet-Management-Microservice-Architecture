
namespace Wallet.Domain.Exceptions
{
    public class CustomerNotFoundException : BaseBusinessException
    {
        public CustomerNotFoundException() : base("Girilen telefon numarasının sistemde kayıtlı ve hesabın aktif olduğundan emin olun.")
        {

        }
    }
}
