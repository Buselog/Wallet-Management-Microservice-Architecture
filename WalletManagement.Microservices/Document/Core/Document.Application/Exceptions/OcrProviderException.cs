
namespace Document.Application.Exceptions
{
    public class OcrProviderException : BaseBusinessException
    {
        public OcrProviderException() : base("ERR_OCR_PROVIDER_UNAVAILABLE")
        {

        }

        public OcrProviderException(string errorDetails) : base("ERR_OCR_PROVIDER_UNAVAILABLE", errorDetails)
        {

        }
    }
}
