
namespace Document.Application.Exceptions
{
    public class FileEmptyException : BaseBusinessException
    {
        public FileEmptyException() : base("ERR_FILE_EMPTY") { }
    }
}
