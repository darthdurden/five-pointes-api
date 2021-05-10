using NodaTime;

namespace FivePointes.Logic.Models
{
    public class Transaction : IModel
    {
        public long Id { get; set; }
        public Account Account { get; set; }
        public string Description { get; set; }
        public LocalDate Date { get; set; }
        public decimal Amount { get; set; }

        public bool IsCleared { get; set; }
        public TransactionCategory Category { get; set; }

        public string Source { get; set; }
        public string SourceId { get; set; }
    }
}
