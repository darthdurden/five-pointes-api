using FivePointes.Api.Dtos;
using FivePointes.Logic.Models;
using FivePointes.Logic.Ports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FivePointes.Api.Controllers.CSVA
{
    [ApiExplorerSettings(GroupName = "csva")]
    [Route("[controller]")]
    [ApiController]
    public class SummaryController : ControllerBase
    {
        private readonly IClientTimeEntriesRepository _clientTimeEntriesRepo;
        private readonly ITimeOffEntriesRepository _timeOffEntriesRepo;
        private readonly IClientsRepository _clientsRepo;
        private readonly IClock _clock;

        public SummaryController(IClientTimeEntriesRepository clientTimeEntriesRepo, ITimeOffEntriesRepository timeOffEntriesRepo, IClientsRepository clientsRepo, IClock clock)
        {
            _clientTimeEntriesRepo = clientTimeEntriesRepo ?? throw new ArgumentNullException(nameof(clientTimeEntriesRepo));
            _timeOffEntriesRepo = timeOffEntriesRepo ?? throw new ArgumentNullException(nameof(timeOffEntriesRepo));
            _clientsRepo = clientsRepo ?? throw new ArgumentNullException(nameof(clientsRepo));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<TimeTrackingSummaryDto>> GetSummary()
        {
            var nowInZone = _clock.GetCurrentInstant().InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault());
            var today = nowInZone.Date;

            var timeFilter = new Logic.Models.Filters.TimeEntryFilter
            {
                Start = DateAdjusters.StartOfMonth(today).AtStartOfDayInZone(DateTimeZoneProviders.Tzdb.GetSystemDefault()).ToInstant()
            };

            var clientTimeEntriesTask = _clientTimeEntriesRepo.GetAsync(timeFilter);
            var timeOffEntriesTask = _timeOffEntriesRepo.GetAsync(timeFilter);
            var clientsTask = _clientsRepo.GetAllAsync();
            var fetchTasks = new List<Task>
            {
                clientTimeEntriesTask,
                timeOffEntriesTask,
                clientsTask
            };

            await Task.WhenAll(fetchTasks);

            if(fetchTasks.All(x => x.IsCompletedSuccessfully))
            {
                var pastHours = 0.0;
                var remainingWorkableHours = 0.0;
                var dayStartRemainingWorkableHours = 0.0;
                var workableHoursToday = 0.0;
                foreach (var date in new DateInterval(DateAdjusters.StartOfMonth(today), DateAdjusters.EndOfMonth(today)))
                {
                    if(date < today)
                    {
                        pastHours += GetWorkingHours(date, timeOffEntriesTask.Result.Value);
                    }
                    else if(date == today)
                    {
                        workableHoursToday = GetWorkingHours(date, timeOffEntriesTask.Result.Value);
                        var futureWorkableHoursToday = GetFutureWorkingHours(date, timeOffEntriesTask.Result.Value);

                        remainingWorkableHours += futureWorkableHoursToday;
                        dayStartRemainingWorkableHours += workableHoursToday;
                        pastHours += workableHoursToday - futureWorkableHoursToday;
                    }
                    else
                    {
                        var futureWorkingHours = GetFutureWorkingHours(date, timeOffEntriesTask.Result.Value);
                        remainingWorkableHours += futureWorkingHours;
                        dayStartRemainingWorkableHours += futureWorkingHours;
                    }
                }

                var clientInfos = new List<TimeTrackingClientInfo>();
                foreach (var client in clientsTask.Result.Value)
                {
                    var timeEntries = clientTimeEntriesTask.Result.Value.Where(x => x.ClientId == client.Id);
                    var timeEntriesToday = timeEntries.Where(x => x.Start != Instant.MinValue && x.Start.InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault()).Date == today);

                    clientInfos.Add(new TimeTrackingClientInfo
                    {
                        Id = client.Id,
                        Name = client.Name,
                        SpentHours = timeEntries.Select(x => x.Duration.TotalHours).DefaultIfEmpty(0).Sum(),
                        TotalCommittedHours = client.Commitment.TotalHours,
                        RemainingWorkableHours = remainingWorkableHours,
                        Colors = timeEntries.Select(x => x.Color).Distinct(),
                        DayStartRemainingWorkableHours = dayStartRemainingWorkableHours,
                        WorkableHoursToday = workableHoursToday,
                        TodaySpentHours = timeEntriesToday.Select(x => x.Duration.TotalHours).DefaultIfEmpty(0).Sum()
                    });
                }

                return new TimeTrackingSummaryDto
                {
                    Clients = clientInfos
                };
            }

            return null; // TODO
        }

        private static double GetWorkingHours(LocalDate date, IEnumerable<TimeEntry> timeOff)
        {
            var hours = 0.0;

            if (date.DayOfWeek == IsoDayOfWeek.Saturday || date.DayOfWeek == IsoDayOfWeek.Sunday)
            {
                return hours;
            }

            hours = 7;

            var startingTime = date.At(new LocalTime(9, 0)).InZoneLeniently(DateTimeZoneProviders.Tzdb.GetSystemDefault()).ToInstant();
            var lunchStartTime = date.At(new LocalTime(12, 0)).InZoneLeniently(DateTimeZoneProviders.Tzdb.GetSystemDefault()).ToInstant();
            var lunchEndTime = date.At(new LocalTime(13, 0)).InZoneLeniently(DateTimeZoneProviders.Tzdb.GetSystemDefault()).ToInstant();
            var quittingTime = date.At(new LocalTime(17, 0)).InZoneLeniently(DateTimeZoneProviders.Tzdb.GetSystemDefault()).ToInstant();

            // Morning Time Off
            var morningTimeOffOnDate = timeOff.Where(x => x.Start <= lunchStartTime && x.End >= startingTime).OrderBy(x => x.Start);
            foreach (var morningTimeOff in morningTimeOffOnDate)
            {
                hours -= (Instant.Min(lunchStartTime, morningTimeOff.End) - Instant.Max(morningTimeOff.Start, startingTime)).TotalHours;
            }

            // Afternoon Time Off
            var afternoonTimeOffOnDate = timeOff.Where(x => x.Start <= quittingTime && x.End >= lunchEndTime).OrderBy(x => x.Start);
            foreach (var afternoonTimeOff in afternoonTimeOffOnDate)
            {
                hours -= (Instant.Min(quittingTime, afternoonTimeOff.End) - Instant.Max(afternoonTimeOff.Start, lunchEndTime)).TotalHours;
            }

            // Don't allow hours to go negative because of overlapping timeoff
            return Math.Max(hours, 0);
        }

        private double GetFutureWorkingHours(LocalDate date, IEnumerable<TimeEntry> timeOff)
        {
            var hours = 0.0;

            if (date.DayOfWeek == IsoDayOfWeek.Saturday || date.DayOfWeek == IsoDayOfWeek.Sunday)
            {
                return hours;
            }

            var startingTime = date.At(new LocalTime(9, 0)).InZoneLeniently(DateTimeZoneProviders.Tzdb.GetSystemDefault()).ToInstant();
            var lunchStartTime = date.At(new LocalTime(12, 0)).InZoneLeniently(DateTimeZoneProviders.Tzdb.GetSystemDefault()).ToInstant();
            var lunchEndTime = date.At(new LocalTime(13, 0)).InZoneLeniently(DateTimeZoneProviders.Tzdb.GetSystemDefault()).ToInstant();
            var quittingTime = date.At(new LocalTime(17, 0)).InZoneLeniently(DateTimeZoneProviders.Tzdb.GetSystemDefault()).ToInstant();

            // Morning
            if(_clock.GetCurrentInstant() < lunchStartTime)
            {
                // The day hasn't started yet
                if (_clock.GetCurrentInstant() < startingTime)
                {
                    hours += 3;
                }
                else
                {
                    hours += (lunchStartTime - _clock.GetCurrentInstant()).TotalHours;
                }

                var morningTimeOffOnDate = timeOff.Where(x => x.Start <= lunchStartTime && x.End >= Instant.Max(startingTime, _clock.GetCurrentInstant())).OrderBy(x => x.Start);
                foreach(var morningTimeOff in morningTimeOffOnDate)
                {
                    hours -= (Instant.Min(lunchStartTime, morningTimeOff.End) - Instant.Max(morningTimeOff.Start, Instant.Max(_clock.GetCurrentInstant(), startingTime))).TotalHours;
                }
            }

            // Afternoon
            if (_clock.GetCurrentInstant() < quittingTime)
            {
                hours += (quittingTime - Instant.Max(_clock.GetCurrentInstant(), lunchEndTime)).TotalHours;

                var afternoonTimeOffOnDate = timeOff.Where(x => x.Start <= quittingTime && x.End >= Instant.Max(lunchEndTime, _clock.GetCurrentInstant())).OrderBy(x => x.Start);
                foreach (var afternoonTimeOff in afternoonTimeOffOnDate)
                {
                    hours -= (Instant.Min(quittingTime, afternoonTimeOff.End) - Instant.Max(afternoonTimeOff.Start, Instant.Max(_clock.GetCurrentInstant(), lunchEndTime))).TotalHours;
                }
            }

            // Don't allow hours to go negative because of overlapping time off
            return Math.Max(hours, 0);
        }
    }
}