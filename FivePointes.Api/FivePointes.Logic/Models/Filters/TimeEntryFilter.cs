using NodaTime;

namespace FivePointes.Logic.Models.Filters
{
    public class TimeEntryFilter
    {
        public Instant? Start { get; set; }
        public Instant? End { get; set; }
    }
}
