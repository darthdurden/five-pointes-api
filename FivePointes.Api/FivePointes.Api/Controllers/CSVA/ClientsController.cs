using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FivePointes.Api.Dtos;
using FivePointes.Common;
using FivePointes.Logic.Models;
using FivePointes.Logic.Ports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FivePointes.Api.Controllers.CSVA
{
    [ApiExplorerSettings(GroupName = "csva")]
    [Route("[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientsRepository _repo;
        private readonly IMapper _mapper;

        public ClientsController(IClientsRepository repo, IMapper mapper)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<List<ClientDto>>> GetClients()
        {
            var clientsResult = await _repo.GetAllAsync();
            if (!clientsResult.IsSuccessful())
            {
                return new ErrorActionResult(clientsResult);
            }

            return _mapper.Map<List<ClientDto>>(clientsResult.Value);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ClientDto>> UpdateClient([FromRoute] string id, [FromBody] ClientDto client)
        {
            var clientResult = await _repo.UpdateAsync(_mapper.Map<ClientDto, Client>(client, opt => opt.AfterMap((src, dest) => dest.Id = id)));
            if (!clientResult.IsSuccessful())
            {
                return new ErrorActionResult(clientResult);
            }

            return _mapper.Map<ClientDto>(clientResult.Value);
        }
    }
}
