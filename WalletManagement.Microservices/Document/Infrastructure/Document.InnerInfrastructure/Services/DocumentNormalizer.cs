using Docnet.Core;
using Docnet.Core.Models;
using Document.Application.Services;
using SkiaSharp;

namespace Document.InnerInfrastructure.Services;

public class DocumentNormalizer : IDocumentNormalizer
{
    public async Task<(byte[] Bytes, string MimeType)> NormalizeToImageAsync(Stream fileStream, string fileName)
    {
        if (fileStream.CanSeek)
        {
            fileStream.Position = 0;
        }

        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        if (extension == ".pdf")
        {
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            var pdfBytes = memoryStream.ToArray();

            using var docReader = DocLib.Instance.GetDocReader(pdfBytes, new PageDimensions(1400, 2000));
            using var pageReader = docReader.GetPageReader(0);

            var rawBytes = pageReader.GetImage();
            var width = pageReader.GetPageWidth();
            var height = pageReader.GetPageHeight();

            var imageInfo = new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
            using var bitmap = new SKBitmap();

            unsafe
            {
                fixed (byte* ptr = rawBytes)
                {
                    bitmap.InstallPixels(imageInfo, (IntPtr)ptr, imageInfo.RowBytes);
                    using var image = SKImage.FromBitmap(bitmap);
                    using var data = image.Encode(SKEncodedImageFormat.Png, 90);
                    return (data.ToArray(), "image/png");
                }
            }
        }

        using var imgStream = new MemoryStream();
        await fileStream.CopyToAsync(imgStream);

        var mimeType = extension switch
        {
            ".png" => "image/png",
            ".jpeg" or ".jpg" => "image/jpeg",
            _ => "image/jpeg"
        };

        return (imgStream.ToArray(), mimeType);
    }
}