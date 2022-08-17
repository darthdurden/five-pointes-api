using FivePointes.Common;
using FivePointes.Logic.Models;
using FivePointes.Logic.Ports;
using System.IO;
using System.Threading.Tasks;
using ImageMagick;

namespace FivePointes.Api.Adapters
{
    public class ImageMagickImageProcessor : IImageProcessor
    {
        public Task<Result<Image>> GetImageAsync(Stream stream)
        {
            int imageWidth;
            int imageHeight;

            using (var image = new MagickImage(stream))
            {
                imageWidth = image.Width;
                imageHeight = image.Height;
            }

            stream.Seek(0, SeekOrigin.Begin);
            return Task.FromResult(Result.Success(new Image
            {
                Data = stream,
                Width = imageWidth,
                Height = imageHeight
            }));
        }

        public Task<Result<Image>> ResizeImageAsync(Image originalImage, int maxWidth, int maxHeight)
        {
            var resizedImage = new MemoryStream();
            int resizedWidth;
            int resizedHeight;

            using (var image = new MagickImage(originalImage.Data))
            {
                var size = new MagickGeometry(maxWidth, maxHeight);
                image.Quality = 100;
                image.Format = MagickFormat.Jpeg;
                image.Resize(size);
                resizedWidth = image.Width;
                resizedHeight = image.Height;
                image.Write(resizedImage);
            }

            resizedImage.Seek(0, SeekOrigin.Begin);
            return Task.FromResult(Result.Success(new Image
            {
                Data = resizedImage,
                Width = resizedWidth,
                Height = resizedHeight
            }));
        }
    }
}
