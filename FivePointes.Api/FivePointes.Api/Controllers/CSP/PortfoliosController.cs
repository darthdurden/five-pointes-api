using AutoMapper;
using FivePointes.Api.Dtos;
using FivePointes.Api.Swagger;
using FivePointes.Common;
using FivePointes.Logic.Models;
using FivePointes.Logic.Ports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FivePointes.Api.Controllers.CSP
{
    [ApiExplorerSettings(GroupName = "csp")]
    [Route("[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class PortfoliosController : ControllerBase
    {
        private readonly IPortfoliosService _service;
        private readonly IMapper _mapper;

        public PortfoliosController(IPortfoliosService service, IMapper mapper)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        #region Public Actions
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        public async Task<ActionResult<List<PortfolioDto>>> GetPortfolios()
        {
            var portfoliosResult = await _service.GetAllAsync();
            if (!portfoliosResult.IsSuccessful())
            {
                return new ErrorActionResult(portfoliosResult);
            }

            return _mapper.Map<List<PortfolioDto>>(portfoliosResult.Value);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        public async Task<ActionResult<PortfolioDto>> GetPortfolio(int id)
        {
            var portfolioResult = await _service.GetAsync(id);
            if (!portfolioResult.IsSuccessful())
            {
                return new ErrorActionResult(portfolioResult);
            }

            return _mapper.Map<PortfolioDto>(portfolioResult.Value);
        }

        [HttpGet("{id}/pictures")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        public async Task<ActionResult<List<PictureDto>>> GetPortfolioPictures(int id)
        {
            var portfolioPicturesResult = await _service.GetPicturesAsync(id, false);
            if (!portfolioPicturesResult.IsSuccessful())
            {
                return new ErrorActionResult(portfolioPicturesResult);
            }

            return _mapper.Map<List<PictureDto>>(portfolioPicturesResult.Value);
        }

        [HttpGet("{id}/pictures/{pictureId}")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        public async Task<ActionResult<PictureDto>> GetPortfolioPicture(int id, int pictureId)
        {
            var portfolioPictureResult = await _service.GetPictureAsync(id, pictureId);
            if (!portfolioPictureResult.IsSuccessful())
            {
                return new ErrorActionResult(portfolioPictureResult);
            }

            return _mapper.Map<PictureDto>(portfolioPictureResult.Value);
        }

        [HttpGet("{id}/pictures/{pictureId}/full.jpg")]
        [AllowAnonymous]
        [ProducesFileResponse(200, "image/jpeg")]
        public async Task<ActionResult<Stream>> GetPortfolioPictureFull(int id, int pictureId)
        {
            var pictureStreamResult = await _service.GetPictureStreamAsync(id, pictureId, PictureSize.Full);
            if (!pictureStreamResult.IsSuccessful())
            {
                return new ErrorActionResult(pictureStreamResult);
            }

            return File(pictureStreamResult.Value, "image/jpeg");
        }

        [HttpGet("{id}/pictures/{pictureId}/thumb.jpg")]
        [AllowAnonymous]
        [ProducesFileResponse(200, "image/jpeg")]
        public async Task<ActionResult<Stream>> GetPortfolioPictureThumbnail(int id, int pictureId)
        {
            var pictureStreamResult = await _service.GetPictureStreamAsync(id, pictureId, PictureSize.Thumbnail);
            if (!pictureStreamResult.IsSuccessful())
            {
                return new ErrorActionResult(pictureStreamResult);
            }

            return File(pictureStreamResult.Value, "image/jpeg");
        }
        #endregion

        #region Admin Actions
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(201)]
        public async Task<ActionResult<PortfolioDto>> CreatePortfolio(PortfolioDto portfolio)
        {
            var portfoliosResult = await _service.CreateAsync(_mapper.Map<Portfolio>(portfolio));
            if (!portfoliosResult.IsSuccessful())
            {
                return new ErrorActionResult(portfoliosResult);
            }

            var created = _mapper.Map<PortfolioDto>(portfoliosResult.Value);

            return Created(created.Links.Self.Href, created);
        }

        [HttpPut("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<PortfolioDto>> UpdatePortfolio(int id, PortfolioDto portfolio)
        {
            var portfoliosResult = await _service.UpdateAsync(_mapper.Map<PortfolioDto, Portfolio>(portfolio, opt => opt.AfterMap((src, dest) => dest.Id = id)));
            if (!portfoliosResult.IsSuccessful())
            {
                return new ErrorActionResult(portfoliosResult);
            }

            return _mapper.Map<PortfolioDto>(portfoliosResult.Value);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        public async Task<ActionResult> DeletePortfolio(int id)
        {
            var portfoliosResult = await _service.DeleteAsync(id);
            if (!portfoliosResult.IsSuccessful())
            {
                return new ErrorActionResult(portfoliosResult);
            }

            return NoContent();
        }

        [HttpPatch("{id}/pictures")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<List<PictureDto>>> PatchPortfolioPictures(int id, IList<PictureDto> pictures)
        {
            var getResult = await _service.GetPicturesAsync(id, true);
            if (!getResult.IsSuccessful())
            {
                return new ErrorActionResult(getResult);
            }

            var updates = new Dictionary<long, PortfolioPicture>();
            var deletions = new List<long>();
            for (var i = 0; i < pictures.Count; i++)
            {
                var picture = pictures[i];
                var existingPic = getResult.Value.SingleOrDefault(x => x.Id == picture.Id);
                if (existingPic != null)
                {
                    updates[picture.Id] = _mapper.Map(picture, existingPic, opt => opt.AfterMap((src, dest) => { dest.SortIndex = i; }));
                } else
                {
                    deletions.Add(picture.Id);
                }
            }

            // Delete any that aren't in the new sort order
            var newIds = pictures.Select(x => (int)x.Id);
            var oldIds = getResult.Value.Select(x => x.Id);

            foreach(var deletePicId in oldIds.Except(newIds))
            {
                await _service.DeletePictureAsync(id, deletePicId);
            }

            var updateResult = await _service.UpdatePicturesAsync(id, updates.Values);
            if (!updateResult.IsSuccessful())
            {
                return new ErrorActionResult(updateResult);
            }

            return _mapper.Map<List<PictureDto>>(updateResult.Value);
        }

        [HttpPost("{id}/pictures")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<List<PictureDto>>> AddPortfolioPicture(int id, [Required] List<IFormFile> pictures)
        {
            var retVal = new List<PictureDto>();

            foreach (var picture in pictures)
            {
                var addResult = await _service.AddPictureAsync(id, picture.OpenReadStream());
                if (!addResult.IsSuccessful())
                {
                    return new ErrorActionResult(addResult);
                }

                retVal.Add(_mapper.Map<PictureDto>(addResult.Value));
            }

            return retVal;
        }

        [HttpDelete("{id}/pictures/{pictureId}")]
        [ProducesResponseType(204)]
        public async Task<ActionResult> DeletePortfolioPicture(int id, int pictureId)
        {
            var deleteResult = await _service.DeletePictureAsync(id, pictureId);
            if (!deleteResult.IsSuccessful())
            {
                return new ErrorActionResult(deleteResult);
            }

            return NoContent();
        }
        #endregion
    }
}
