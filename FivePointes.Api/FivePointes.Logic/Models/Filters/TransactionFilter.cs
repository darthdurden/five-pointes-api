using NodaTime;
using System.Collections.Generic;

namespace FivePointes.Logic.Models.Filters
{
    public class TransactionFilter
    {
        public LocalDate? StartDate { get; set; }
        public LocalDate? EndDate { get; set; }
        public bool? IsCleared { get; set; }
        public bool? IsWrittenOff { get; set; }
        public long? AccountId { get; set; }
        public long? CategoryId { get; set; }
        public IEnumerable<TransactionType> Types { get; set; }
    }
}
