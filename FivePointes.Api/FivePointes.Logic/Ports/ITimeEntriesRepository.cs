using FivePointes.Common;
using FivePointes.Logic.Models;
using FivePointes.Logic.Models.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FivePointes.Logic.Ports
{
    public interface ITimeEntriesRepository<TTimeEntry> where TTimeEntry : TimeEntry
    {
        public Task<Result<IEnumerable<TTimeEntry>>> GetAsync(TimeEntryFilter filter);
    }
}
