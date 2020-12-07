using AutoMapper;
using Clockify.Net;
using Clockify.Net.Models.TimeEntries;
using FivePointes.Api.Configuration;
using FivePointes.Common;
using FivePointes.Logic.Models;
using FivePointes.Logic.Models.Filters;
using FivePointes.Logic.Ports;
using Microsoft.Extensions.Options;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FivePointes.Api.Adapters
{
    public class ClockifyClientTimeEntriesRepository : IClientTimeEntriesRepository
    {
        private readonly ClockifyClient _clockifyClient;
        private readonly IMapper _mapper;
        private readonly ClockifyOptions _options;

        public ClockifyClientTimeEntriesRepository(ClockifyClient clockifyClient, IMapper mapper, IOptions<ClockifyOptions> options)
        {
            _clockifyClient = clockifyClient ?? throw new ArgumentNullException(nameof(clockifyClient));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<Result<IEnumerable<ClientTimeEntry>>> GetAsync(TimeEntryFilter filter)
        {
            var timeEntriesTask = _clockifyClient.FindAllTimeEntriesForUserAsync(
                workspaceId: _options.WorkspaceId,
                userId: _options.UserId,
                start: filter.Start?.ToDateTimeOffset(),
                end: filter.End?.ToDateTimeOffset(),
                pageSize: 1000);

            var projectsTask = _clockifyClient.FindAllProjectsOnWorkspaceAsync(
                workspaceId: _options.WorkspaceId,
                archived: false,
                pageSize: 1000);

            var clockifyTasks = new List<Task>
            {
                timeEntriesTask,
                projectsTask
            };

            await Task.WhenAll(clockifyTasks);

            // TODO error handling

            var clientTimeEntries = new List<ClientTimeEntry>();
            foreach(var clockifyEntry in timeEntriesTask.Result.Data)
            {
                clientTimeEntries.Add(_mapper.Map<TimeEntryDtoImpl, ClientTimeEntry>(clockifyEntry, opt => opt.AfterMap((src, dest) =>
                {
                    var project = projectsTask.Result.Data.FirstOrDefault(x => x.Id == src.ProjectId);
                    if (project != null)
                    {
                        dest.ClientId = project.ClientId;
                        dest.Color = project.Color;
                    }
                })));
            }

            // Add in some dummy entries with no time to make sure we get all the colors
            clientTimeEntries.AddRange(projectsTask.Result.Data.Select(x => new ClientTimeEntry
            {
                ClientId = x.ClientId,
                Color = x.Color,
                Description = x.Name,
                Start = Instant.MinValue,
                End = Instant.MinValue
            }));

            return Result.Success(clientTimeEntries.AsEnumerable());
        }
    }
}
