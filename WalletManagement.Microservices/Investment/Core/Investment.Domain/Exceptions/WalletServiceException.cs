namespace Investment.Domain.Exceptions
{
    public class WalletServiceException : BaseBusinessException
    {
        public int StatusCode { get; }
        public WalletServiceException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
