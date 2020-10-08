using FivePointes.Api.Dtos.Hal;
using FivePointes.Logic.Models;
using NodaTime;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace FivePointes.Api.Dtos
{
    public class TransactionDto
    {
        [JsonPropertyName("_links")]
        public TransactionHalLinks Links { get; set; }
        [ReadOnly(true)]
        public long Id { get; private set; }
        public string Description { get; set; }
        public LocalDate Date { get; set; }
        public decimal Amount { get; set; }
        public bool IsCleared { get; set; }
        public long? AccountId { get; set; }
        public long? CategoryId { get; set; }
        public TransactionType? Type { get; internal set; }
    }
}
