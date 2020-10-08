using FivePointes.Api.Dtos.Hal;
using NodaTime;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace FivePointes.Api.Dtos
{
    public class TimeOffEntryDto
    {
        [JsonPropertyName("_links")]
        public HalLinks Links { get; set; }
        [ReadOnly(true)]
        public long Id { get; private set; }
        public Instant Start { get; set; }
        public Instant End { get; set; }
        public string Description { get; set; }
    }
}
