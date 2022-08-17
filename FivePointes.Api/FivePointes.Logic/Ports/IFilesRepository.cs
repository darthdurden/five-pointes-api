using FivePointes.Common;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FivePointes.Logic.Ports
{
    public interface IFilesRepository
    {
        Task<Result<Stream>> GetFileAsync(string root, string relativePath);
        Task<Result> SaveFileAsync(string root, string relativePath, Stream stream);
        Task<Result> DeleteFileAsync(string root, string relativePath);
        Task<Result> DeleteFilesAsync(string root, IEnumerable<string> relativePaths);
    }
}
