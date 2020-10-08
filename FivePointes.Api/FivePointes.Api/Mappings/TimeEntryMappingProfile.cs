using AutoMapper;
using Clockify.Net.Models.TimeEntries;
using FivePointes.Api.Controllers;
using FivePointes.Api.Controllers.CSVA;
using FivePointes.Api.Dtos;
using FivePointes.Api.Dtos.Hal;
using FivePointes.Api.Mappings.Resolvers;
using FivePointes.Data.Models;
using FivePointes.Logic.Models;
using NodaTime;
using System.Collections.Generic;

namespace FivePointes.Api.Mappings
{
    public class TimeEntryMappingProfile : Profile
    {
        public TimeEntryMappingProfile()
        {
            CreateMap<TimeEntryDtoImpl, ClientTimeEntry>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.Start, opt => opt.MapFrom(src => src.TimeInterval.Start))
                .ForMember(x => x.End, opt => opt.MapFrom(src => src.TimeInterval.End.HasValue ? Instant.FromDateTimeOffset(src.TimeInterval.End.Value) : SystemClock.Instance.GetCurrentInstant()));

            CreateMap<TimeOffEntryDto, TimeEntry>()
                .ReverseMap()
                .ForMember(m => m.Links, opt => opt.MapFrom(src => src));

            CreateMap<TimeEntry, TimeOffEntry>()
                .ReverseMap();

            CreateMap<TimeEntry, HalLinks>()
                .ForMember(m => m.Self, opt => opt.MapFrom<HalLinkResolver, HalLinkContext>(src => new HalLinkContext
                {
                    ControllerType = typeof(TimeOffEntriesController),
                    MethodName = nameof(TimeOffEntriesController.GetTimeOffEntry),
                    PathParameters = new Dictionary<string, string>
                    {
                        { "id", $"{src.Id}" }
                    }
                }));
        }
    }
}
