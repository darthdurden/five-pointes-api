using AutoMapper;
using NodaTime;
using System;

namespace FivePointes.Api.Mappings
{
    public class NodaTimeMappingProfile : Profile
    {
        public NodaTimeMappingProfile()
        {
            CreateMap<DateTime, Instant>()
                .ConstructUsing((src, context) =>
                {
                    switch (src.Kind)
                    {
                        case DateTimeKind.Utc:
                            return Instant.FromDateTimeUtc(src);
                        case DateTimeKind.Local:
                            return LocalDateTime.FromDateTime(src).InZoneLeniently(DateTimeZoneProviders.Tzdb.GetSystemDefault()).ToInstant();
                        case DateTimeKind.Unspecified:
                        default:
                            return Instant.FromDateTimeUtc(src);
                    }
                });

            CreateMap<Instant, DateTime>()
                .ConstructUsing((src, context) => src.ToDateTimeUtc());

            CreateMap<DateTime, LocalDate>()
                .ConstructUsing((src, context) => LocalDate.FromDateTime(src));

            CreateMap<LocalDate, DateTime>()
                .ConstructUsing((src, context) => src.ToDateTimeUnspecified());

            CreateMap<DateTimeOffset, Instant>()
                .ConvertUsing((src, context) => Instant.FromDateTimeOffset(src));

            CreateMap<Instant, DateTimeOffset>()
                .ConvertUsing((src, context) => src.ToDateTimeOffset());
        }
    }
}
