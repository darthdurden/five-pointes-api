﻿using System;
using System.Collections.Generic;

namespace FivePointes.Api.Dtos
{
    public class TimeTrackingClientInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double SpentHours { get; set; }
        public double TotalCommittedHours { get; set; }
        public double RemainingWorkableHours { get; set; }
        public double DayStartRemainingWorkableHours { get; set; }
        public double RemainingWorkableDays => RemainingWorkableHours / 7.0;
        public double DayStartRemainingWorkableDays => DayStartRemainingWorkableHours / 7.0;
        public double WorkableHoursToday { get; set; }
        public double TodaySpentHours { get; set; }
        public double TodayOvertimeHours => Math.Min(OvertimeHours, TodaySpentHours);
        public double RemainingCommittedHours => Math.Max(0, TotalCommittedHours - SpentHours);
        public double OvertimeHours => Math.Max(0, SpentHours - TotalCommittedHours);
        public double AverageHoursPerDayRemaining => RemainingWorkableDays > 0 ? RemainingCommittedHours / RemainingWorkableDays : 0;
        public double DayStartAverageHoursPerDayRemaining => (DayStartRemainingWorkableDays > 0 ? (RemainingCommittedHours + (TodaySpentHours - TodayOvertimeHours)) / DayStartRemainingWorkableDays : 0) * (WorkableHoursToday / 7.0);
        public IEnumerable<string> Colors { get; set; }
        public bool IsHidden { get; set; }
    }
}
