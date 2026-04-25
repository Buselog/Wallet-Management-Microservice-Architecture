namespace Wallet.Domain.Exceptions
{
    public class BaseBusinessException : Exception
    {
        public object[] Parameters { get; }
        public BaseBusinessException(string message, params object[] parameters) : base(message) {

            Parameters = parameters;
        }
    }
}
