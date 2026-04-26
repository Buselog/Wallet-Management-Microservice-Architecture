namespace Investment.Domain.Exceptions
{
    public class WalletServiceException : BaseBusinessException
    {
        public int StatusCode { get; }
        public WalletServiceException(int statusCode, string message, params object[] parameters) : base(message, parameters)
        {
            StatusCode = statusCode;
        }
    }
}
