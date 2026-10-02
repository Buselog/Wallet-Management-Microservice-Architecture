namespace Wallet.Domain.Exceptions
{
    public class ReferenceAlreadyExistsException : BaseBusinessException
    {
        public const string code = "ERR_REFERENCE_ALREADY_EXIST";
        public ReferenceAlreadyExistsException() : base(code)
        {

        }
    }
}