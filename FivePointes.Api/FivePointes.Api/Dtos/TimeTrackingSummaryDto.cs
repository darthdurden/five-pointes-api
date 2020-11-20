using System.Collections.Generic;
using System.Linq;

namespace FivePointes.Api.Dtos
{
    public class TimeTrackingSummaryDto
    {
        public double TodaySpentHours => Clients.Sum(x => x.TodaySpentHours);
        public double TodayOvertimeHours => Clients.Sum(x => x.TodayOvertimeHours);
        public double TotalCommittedHours => Clients.Sum(x => x.TotalCommittedHours);
        public double RemainingCommittedHours => Clients.Sum(x => x.RemainingCommittedHours);
        public double OvertimeHours => Clients.Sum(x => x.OvertimeHours);
        public double AverageHoursPerDayRemaining => Clients.Sum(x => x.AverageHoursPerDayRemaining);
        public double DayStartAverageHoursPerDayRemaining => Clients.Sum(x => x.DayStartAverageHoursPerDayRemaining);
        public IEnumerable<TimeTrackingClientInfo> Clients { get; set; }
    }
}
