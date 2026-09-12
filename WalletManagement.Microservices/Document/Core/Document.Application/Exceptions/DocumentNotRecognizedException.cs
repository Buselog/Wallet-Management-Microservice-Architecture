
namespace Document.Application.Exceptions
{
    public class DocumentNotRecognizedException : BaseBusinessException
    {
        public DocumentNotRecognizedException() : base("ERR_DOCUMENT_NOT_RECOGNIZED") { }
    }
}
