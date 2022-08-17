using FivePointes.Common;
using FivePointes.Logic.Ports;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace FivePointes.Api.Adapters
{
    public class LocalFilesRepository : IFilesRepository
    {
        public Task<Result<Stream>> GetFileAsync(string root, string relativePath)
        {
            var absolutePath = Path.Combine(root, relativePath);
            if (!File.Exists(absolutePath))
            {
                return Task.FromResult(Result.Error<Stream>(HttpStatusCode.NotFound));
            }

            return Task.FromResult(Result.Success<Stream>(File.OpenRead(absolutePath)));
        }

        public Task<Result> SaveFileAsync(string root, string relativePath, Stream stream)
        {
            using (var fileStream = File.Create(Path.Combine(root, relativePath)))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
            }

            return Task.FromResult(Result.Success());
        }

        public Task<Result> DeleteFileAsync(string root, string relativePath)
        {
            var absolutePath = Path.Combine(root, relativePath);
            if (File.Exists(absolutePath))
            {
                File.Delete(absolutePath);
            }

            return Task.FromResult(Result.Success());
        }

        public Task<Result> DeleteFilesAsync(string root, IEnumerable<string> relativePaths)
        {
            foreach (var relativePath in relativePaths)
            {
                DeleteFileAsync(root, relativePath);
            }

            return Task.FromResult(Result.Success());
        }
    }
}
