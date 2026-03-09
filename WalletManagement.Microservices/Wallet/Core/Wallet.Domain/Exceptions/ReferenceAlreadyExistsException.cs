

namespace Wallet.Domain.Exceptions
{
    public class ReferenceAlreadyExistsException : BaseBusinessException
    {
        public ReferenceAlreadyExistsException() : base("Bu referans numarası ile daha önce işlem yapılmış.")
        {

        }
    }
}