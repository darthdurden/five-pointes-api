using NodaTime;
using System;

namespace FivePointes.Api
{
    public class TestClock : IClock
    {
        public Instant GetCurrentInstant()
        {
            return new LocalDateTime(2020, 11, 19, 5, 0).InZoneLeniently(DateTimeZoneProviders.Tzdb.GetSystemDefault()).ToInstant();
        }
    }
}
