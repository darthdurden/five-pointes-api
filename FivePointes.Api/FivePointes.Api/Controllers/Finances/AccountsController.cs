using AutoMapper;
using FivePointes.Common;
using FivePointes.Api.Dtos;
using FivePointes.Logic.Models;
using FivePointes.Logic.Ports;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FivePointes.Logic.Models.Filters;

namespace FivePointes.Api.Controllers.Finances
{
    [ApiExplorerSettings(GroupName = "finances")]
    [Route("[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountsService _service;
        private readonly IMapper _mapper;

        public AccountsController(IAccountsService service, IMapper mapper)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<List<AccountDto>>> GetAccounts()
        {
            var result = await _service.GetAsync(default(AccountFilter));
            if (!result.IsSuccessful())
            {
                return new ErrorActionResult(result);
            }

            return _mapper.Map<List<AccountDto>>(result.Value);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<AccountDto>> GetAccount(long id)
        {
            var result = await _service.GetAsync(id);
            if (!result.IsSuccessful())
            {
                return new ErrorActionResult(result);
            }

            return _mapper.Map<AccountDto>(result.Value);
        }

        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(201)]
        public async Task<ActionResult<AccountDto>> CreateAccount(AccountDto account)
        {
            var result = await _service.CreateAsync(_mapper.Map<Account>(account));
            if (!result.IsSuccessful())
            {
                return new ErrorActionResult(result);
            }

            var created = _mapper.Map<AccountDto>(result.Value);

            return Created(created.Links.Self.Href, created);
        }
    }
}