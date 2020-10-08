using FivePointes.Api.Dtos.Hal;
using System.Text.Json.Serialization;

namespace FivePointes.Api.Dtos
{
    public class PictureDto
    {
        [JsonPropertyName("_links")]
        public PictureHalLinks Links { get; private set; }

        public long Id { get; set; } 
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int ThumbnailWidth { get; private set; }
        public int ThumbnailHeight { get; private set; }
    }
}
