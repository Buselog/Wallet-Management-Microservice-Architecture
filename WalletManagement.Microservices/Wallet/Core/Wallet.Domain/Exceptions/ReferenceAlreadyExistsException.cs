

namespace Wallet.Domain.Exceptions
{
    public class ReferenceAlreadyExistsException : BaseBusinessException
    {
        public ReferenceAlreadyExistsException() : base("ERR_REFERENCE_ALREADY_EXIST")
        {

        }
    }
}