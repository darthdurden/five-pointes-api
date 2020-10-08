using NodaTime;

namespace FivePointes.Logic.Models
{
    public class TimeEntry : IModel
    {
        public long Id { get; set; }
        public Duration Duration => End - Start;
        public Instant Start { get; set; }
        public Instant End { get; set; }
        public string Description { get; set; }
    }
}
