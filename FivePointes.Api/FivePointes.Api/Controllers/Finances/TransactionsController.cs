using AutoMapper;
using FivePointes.Api.Dtos;
using FivePointes.Common;
using FivePointes.Logic.Models;
using FivePointes.Logic.Models.Filters;
using FivePointes.Logic.Ports;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FivePointes.Api.Controllers.Finances
{
    [ApiExplorerSettings(GroupName = "finances")]
    [Route("[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionsService _service;
        private readonly IMapper _mapper;

        public TransactionsController(ITransactionsService service, IMapper mapper)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<TransactionDto>> GetTransaction(long id)
        {
            var result = await _service.GetAsync(id);
            if (!result.IsSuccessful())
            {
                return new ErrorActionResult(result);
            }

            return _mapper.Map<TransactionDto>(result.Value);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        public async Task<ActionResult<TransactionDto>> DeleteTransaction(long id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result.IsSuccessful())
            {
                return new ErrorActionResult(result);
            }

            return NoContent();
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<List<TransactionDto>>> GetTransactions([FromQuery]TransactionFilter filter)
        {
            if(!filter.StartDate.HasValue && !filter.EndDate.HasValue)
            {
                filter.StartDate = LocalDate.FromDateTime(DateTime.Now.Date.AddDays(-30));
            }

            var result = await _service.GetAsync(filter);
            if (!result.IsSuccessful())
            {
                return new ErrorActionResult(result);
            }

            return _mapper.Map<List<TransactionDto>>(result.Value);
        }

        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(201)]
        public async Task<ActionResult<PortfolioDto>> CreateTransaction(TransactionDto transaction)
        {
            var result = await _service.CreateAsync(_mapper.Map<TransactionDto, Transaction>(transaction, opt => opt.AfterMap((src, dest) =>
            {
                if (src.AccountId.HasValue)
                {
                    dest.Account = new Account { Id = src.AccountId.Value };
                }

                if (src.CategoryId.HasValue)
                {
                    dest.Category = new TransactionCategory { Id = src.CategoryId.Value };
                }
            })));
            if (!result.IsSuccessful())
            {
                return new ErrorActionResult(result);
            }

            var created = _mapper.Map<TransactionDto>(result.Value);

            return Created(created.Links.Self.Href, created);
        }

        [HttpPut("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<TransactionDto>> UpdateTransaction(long id, TransactionDto transaction)
        {
            var result = await _service.UpdateAsync(_mapper.Map<TransactionDto, Transaction>(transaction, opt => opt.AfterMap((src, dest) => { 
                dest.Id = id;
                if (src.AccountId.HasValue)
                {
                    dest.Account = new Account { Id = src.AccountId.Value };
                }

                if (src.CategoryId.HasValue)
                {
                    dest.Category = new TransactionCategory { Id = src.CategoryId.Value };
                }
            })));

            if (!result.IsSuccessful())
            {
                return new ErrorActionResult(result);
            }

            return _mapper.Map<TransactionDto>(result.Value);
        }
    }
}