using AutoMapper;
using Clockify.Net;
using Clockify.Net.Models.Clients;
using FivePointes.Api.Configuration;
using FivePointes.Common;
using FivePointes.Data;
using FivePointes.Logic.Models;
using FivePointes.Logic.Models.Filters;
using FivePointes.Logic.Ports;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FivePointes.Api.Adapters
{
    public class ClockifyClientsRepository : IClientsRepository
    {
        private readonly ClockifyClient _clockifyClient;
        private readonly IMapper _mapper;
        private readonly ClockifyOptions _options;
        private readonly FivePointesDbContext _context;

        public ClockifyClientsRepository(ClockifyClient clockifyClient, FivePointesDbContext context, IMapper mapper, IOptions<ClockifyOptions> options)
        {
            _clockifyClient = clockifyClient ?? throw new ArgumentNullException(nameof(clockifyClient));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<Result<IEnumerable<Client>>> GetAllAsync()
        {
            var clientsTask = _clockifyClient.FindAllClientsOnWorkspaceAsync(
                workspaceId: _options.WorkspaceId
                );

            var commitmentsTask = _context.Commitments.ToListAsync();

            var initTasks = new List<Task>
            {
                clientsTask,
                commitmentsTask
            };

            await Task.WhenAll(initTasks);

            // TODO handle errors

            var addedCommitments = false;

            var clients = new List<Client>();
            foreach(var clockifyClient in clientsTask.Result.Data)
            {
                var commitment = commitmentsTask.Result.FirstOrDefault(x => x.ClientId == clockifyClient.Id);
                if(commitment == null)
                {
                    commitment = new Data.Models.Commitment
                    {
                        ClientId = clockifyClient.Id,
                        AmountInHours = 0
                    };

                    await _context.Commitments.AddAsync(commitment);
                    addedCommitments = true;
                }

                clients.Add(_mapper.Map<ClientDto, Client>(clockifyClient, opt => opt.AfterMap((src, dest) => {
                    dest.Commitment = Duration.FromHours(commitment.AmountInHours);
                    dest.IsHidden = commitment.IsHidden;
                })));
            }

            if (addedCommitments)
            {
                await _context.SaveChangesAsync();
            }

            // TODO error handling

            return Result.Success<IEnumerable<Client>>(clients);
        }

        public async Task<Result<Client>> UpdateAsync(Client client)
        {
            var allClients = await _clockifyClient.FindAllClientsOnWorkspaceAsync(_options.WorkspaceId);
            var clockifyClient = allClients.Data.FirstOrDefault(x => x.Id == client.Id);

            if(clockifyClient == null)
            {
                return Result.Error<Client>(System.Net.HttpStatusCode.NotFound);
            }

            var commitment = await _context.Commitments.FindAsync(client.Id);
            if(commitment == null)
            {
                commitment = new Data.Models.Commitment
                {
                    ClientId = client.Id
                };
                _context.Commitments.Attach(commitment);
            }

            commitment.AmountInHours = client.Commitment.TotalHours;
            commitment.IsHidden = client.IsHidden;
            await _context.SaveChangesAsync();

            return Result.Success(_mapper.Map<ClientDto, Client>(clockifyClient, opt => opt.AfterMap((src, dest) => {
                dest.Commitment = Duration.FromHours(commitment.AmountInHours);
                dest.IsHidden = commitment.IsHidden;
            })));
        }
    }
}
