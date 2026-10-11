
namespace Document.Application.Services
{
    public interface IDocumentNormalizer
    {
        Task<(byte[] Bytes, string MimeType)> NormalizeToImageAsync(Stream fileStream, string fileName);
    }
}
