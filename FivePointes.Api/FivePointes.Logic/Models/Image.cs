using System.IO;

namespace FivePointes.Logic.Models
{
    public class Image
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public Stream Data { get; set; }
    }
}
