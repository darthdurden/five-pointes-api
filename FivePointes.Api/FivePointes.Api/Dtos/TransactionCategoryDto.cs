using FivePointes.Api.Dtos.Hal;
using FivePointes.Logic.Models;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace FivePointes.Api.Dtos
{
    public class TransactionCategoryDto
    {
        [JsonPropertyName("_links")]
        public HalLinks Links { get; set; }
        [ReadOnly(true)]
        public long Id { get; set; }
        public string Name { get; set; }
        public TransactionType TransactionType { get; set; }
    }
}
