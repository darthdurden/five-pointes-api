using AutoMapper;
using FivePointes.Api.Dtos;
using FivePointes.Common;
using FivePointes.Logic.Models;
using FivePointes.Logic.Models.Filters;
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
    public class TimeOffEntriesController : ControllerBase
    {
        private readonly ITimeOffEntriesRepository _service;
        private readonly IMapper _mapper;

        public TimeOffEntriesController(ITimeOffEntriesRepository service, IMapper mapper)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<TimeOffEntryDto>> GetTimeOffEntry(long id)
        {
            var result = await _service.GetAsync(id);
            if (!result.IsSuccessful())
            {
                return new ErrorActionResult(result);
            }

            return _mapper.Map<TimeOffEntryDto>(result.Value);
        }

        [HttpDelete("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(204)]
        public async Task<ActionResult<TimeOffEntryDto>> DeleteTimeOffEntry(long id)
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
        [AllowAnonymous]
        public async Task<ActionResult<List<TimeOffEntryDto>>> GetTimeOffEntries([FromQuery]TimeEntryFilter filter)
        {
            if (!filter.Start.HasValue && !filter.End.HasValue)
            {
                filter.Start = DateAdjusters.StartOfMonth(LocalDate.FromDateTime(DateTime.Now.Date)).AtStartOfDayInZone(DateTimeZoneProviders.Tzdb.GetSystemDefault()).ToInstant();
                filter.End = DateAdjusters.EndOfMonth(LocalDate.FromDateTime(DateTime.Now.Date)).At(LocalTime.MaxValue).InZoneLeniently(DateTimeZoneProviders.Tzdb.GetSystemDefault()).ToInstant();
            }

            var result = await _service.GetAsync(filter);
            if (!result.IsSuccessful())
            {
                return new ErrorActionResult(result);
            }

            return _mapper.Map<List<TimeOffEntryDto>>(result.Value.OrderBy(x => x.Start));
        }

        [HttpPost]
        [AllowAnonymous]
        [Consumes("application/json")]
        [ProducesResponseType(201)]
        public async Task<ActionResult<TimeOffEntryDto>> CreateTimeOffEntry(TimeOffEntryDto timeOffEntry)
        {
            var result = await _service.CreateAsync(_mapper.Map<TimeEntry>(timeOffEntry));
            if (!result.IsSuccessful())
            {
                return new ErrorActionResult(result);
            }

            var created = _mapper.Map<TimeOffEntryDto>(result.Value);

            return Created(created.Links.Self.Href, created);
        }

        [HttpPut("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<TimeOffEntryDto>> UpdateTimeOffEntry(long id, TimeOffEntryDto timeOffEntry)
        {
            var result = await _service.UpdateAsync(_mapper.Map<TimeOffEntryDto, TimeEntry>(timeOffEntry, opt => opt.AfterMap((src, dest) => {
                dest.Id = id;
            })));

            if (!result.IsSuccessful())
            {
                return new ErrorActionResult(result);
            }

            return _mapper.Map<TimeOffEntryDto>(result.Value);
        }
    }
}