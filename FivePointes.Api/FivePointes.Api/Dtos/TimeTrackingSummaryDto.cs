using System.Collections.Generic;
using System.Linq;

namespace FivePointes.Api.Dtos
{
    public class TimeTrackingSummaryDto
    {
        public double SpentHours => Clients.Sum(x => x.SpentHours);
        public double TotalCommittedHours => Clients.Sum(x => x.TotalCommittedHours);
        public double RemainingCommittedHours => Clients.Sum(x => x.RemainingCommittedHours);
        public double OvertimeHours => Clients.Sum(x => x.OvertimeHours);
        public double RemainingWorkableHours { get; set; }
        public double PastWorkableHours { get; set; }
        public double PastWorkableDays => PastWorkableHours / 7.0;
        public double RemainingWorkableDays => RemainingWorkableHours / 7.0;
        public double AverageHoursPerDayRemaining => RemainingWorkableDays > 0 ? RemainingCommittedHours / RemainingWorkableDays : 0;
        public IEnumerable<TimeTrackingClientInfo> Clients { get; set; }
    }
}
