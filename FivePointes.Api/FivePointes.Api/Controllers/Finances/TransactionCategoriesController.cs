using AutoMapper;
using FivePointes.Api.Dtos;
using FivePointes.Common;
using FivePointes.Logic.Models;
using FivePointes.Logic.Models.Filters;
using FivePointes.Logic.Ports;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FivePointes.Api.Controllers.Finances
{
    [ApiExplorerSettings(GroupName = "finances")]
    [Route("[controller]")]
    [ApiController]
    public class TransactionCategoriesController : ControllerBase
    {
        private readonly ITransactionCategoriesService _service;
        private readonly IMapper _mapper;

        public TransactionCategoriesController(ITransactionCategoriesService service, IMapper mapper)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<TransactionCategoryDto>> GetTransactionCategory(long id)
        {
            var result = await _service.GetAsync(id);
            if (!result.IsSuccessful())
            {
                return new ErrorActionResult(result);
            }

            return _mapper.Map<TransactionCategoryDto>(result.Value);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        public async Task<ActionResult<TransactionCategoryDto>> DeleteTransactionCategory(long id)
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
        public async Task<ActionResult<List<TransactionCategoryDto>>> GetTransactionCategories()
        {
            var result = await _service.GetAsync(default(TransactionCategoryFilter));
            if (!result.IsSuccessful())
            {
                return new ErrorActionResult(result);
            }

            return _mapper.Map<List<TransactionCategoryDto>>(result.Value);
        }

        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(201)]
        public async Task<ActionResult<PortfolioDto>> CreateTransactionCategory(TransactionCategoryDto transactionCategory)
        {
            var result = await _service.CreateAsync(_mapper.Map<TransactionCategory>(transactionCategory));
            if (!result.IsSuccessful())
            {
                return new ErrorActionResult(result);
            }

            var created = _mapper.Map<TransactionCategoryDto>(result.Value);

            return Created(created.Links.Self.Href, created);
        }

        [HttpPut("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<TransactionCategoryDto>> UpdateTransactionCategory(long id, TransactionCategoryDto transactionCategory)
        {
            var result = await _service.UpdateAsync(_mapper.Map<TransactionCategoryDto, TransactionCategory>(transactionCategory, opt => opt.AfterMap((src, dest) => dest.Id = id)));
            if (!result.IsSuccessful())
            {
                return new ErrorActionResult(result);
            }

            return _mapper.Map<TransactionCategoryDto>(result.Value);
        }
    }
}