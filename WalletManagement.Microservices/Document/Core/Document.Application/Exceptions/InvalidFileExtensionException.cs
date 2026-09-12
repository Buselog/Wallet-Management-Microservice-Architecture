
namespace Document.Application.Exceptions
{
    public class InvalidFileExtensionException : BaseBusinessException
    {
        public InvalidFileExtensionException() : base("ERR_INVALID_FILE_EXTENSION") { }
    }
}
