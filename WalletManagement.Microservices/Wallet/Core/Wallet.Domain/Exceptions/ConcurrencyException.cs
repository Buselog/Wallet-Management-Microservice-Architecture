
namespace Wallet.Domain.Exceptions
{
    public class ConcurrencyException : BaseBusinessException
    {
        public ConcurrencyException() : base("İşlem sırasında bir çakışma oldu, lütfen tekrar deneyin.")
        {

        }
    }
}
