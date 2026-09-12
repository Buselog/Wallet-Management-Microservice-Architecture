
namespace Document.Application.Exceptions
{
    public class FileSizeExceededException : BaseBusinessException
    {
        public FileSizeExceededException() : base("ERR_FILE_SIZE_EXCEEDED") { }
    }
}
