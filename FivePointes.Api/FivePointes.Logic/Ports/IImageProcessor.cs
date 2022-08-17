using FivePointes.Common;
using FivePointes.Logic.Models;
using System.IO;
using System.Threading.Tasks;

namespace FivePointes.Logic.Ports
{
    public interface IImageProcessor
    {
        Task<Result<Image>> GetImageAsync(Stream stream);
        Task<Result<Image>> ResizeImageAsync(Image originalImage, int maxWidth, int maxHeight);
    }
}
