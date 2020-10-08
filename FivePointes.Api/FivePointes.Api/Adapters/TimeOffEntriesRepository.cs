using AutoMapper;
using FivePointes.Data;
using FivePointes.Logic.Models;
using FivePointes.Logic.Models.Filters;
using FivePointes.Logic.Ports;

namespace FivePointes.Api.Adapters
{
    public class TimeOffEntriesRepository : CrudRepository<TimeEntry, TimeEntryFilter, Data.Models.TimeOffEntry>, ITimeOffEntriesRepository
    {
        public TimeOffEntriesRepository(FivePointesDbContext context, IMapper mapper) : base(mapper, context, context.TimeOffEntries) { }
    }
}
