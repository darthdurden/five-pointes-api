using FivePointes.Api.Dtos.Hal;
using System.Text.Json.Serialization;

namespace FivePointes.Api.Dtos
{
    public class AccountDto
    {
        [JsonPropertyName("_links")]
        public AccountHalLinks Links { get; set; }
        public long Id { get; private set; }
        public string Name { get; set; }
        public decimal Total { get; private set; }
        public decimal ClearedTotal { get; private set; }
    }
}
