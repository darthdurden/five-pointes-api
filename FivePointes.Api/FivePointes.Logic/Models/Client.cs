using NodaTime;

namespace FivePointes.Logic.Models
{
    public class Client
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Duration Commitment { get; set; }
    }
}
