using FivePointes.Api.Dtos.Hal;
using System.Text.Json.Serialization;

namespace FivePointes.Api.Dtos
{
    public class PortfolioDto
    {
        public long Id { get; private set; }
        public string Name { get; set; }
        [JsonPropertyName("_links")]
        public PortfolioHalLinks Links { get; private set; }
    }
}
