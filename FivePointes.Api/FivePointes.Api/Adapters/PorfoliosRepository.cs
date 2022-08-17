using AutoMapper;
using FivePointes.Common;
using FivePointes.Data;
using FivePointes.Logic.Models;
using FivePointes.Logic.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FivePointes.Api.Adapters
{
    public class PortfoliosRepository : IPortfoliosRepository
    {
        private readonly FivePointesDbContext _context;
        private readonly IMapper _mapper;

        public PortfoliosRepository(FivePointesDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result<IEnumerable<Portfolio>>> GetAllAsync()
        {
            try
            {
                var portfoliosQuery = _context.Portfolios;
                var portfolios = await portfoliosQuery.ToListAsync();

                return Result.Success(_mapper.Map<IEnumerable<Portfolio>>(portfolios));
            }
            catch (Exception e)
            {
                return Result.Exception<IEnumerable<Portfolio>>(e);
            }
        }

        public async Task<Result<Portfolio>> GetAsync(int id)
        {
            try
            {
                var portfolio = await _context.Portfolios.FindAsync(id);
                if (portfolio == null)
                {
                    return Result.Error<Portfolio>(HttpStatusCode.NotFound);
                }

                return Result.Success(_mapper.Map<Portfolio>(portfolio));
            }
            catch (Exception e)
            {
                return Result.Exception<Portfolio>(e);
            }
        }

        public async Task<Result<IEnumerable<PortfolioPicture>>> GetPicturesAsync(int id)
        {
            try
            {
                var pictures = await _context.PortfolioPictures.Where(x => x.PortfolioId == id).OrderBy(x => x.SortIndex).ToListAsync();
                return Result.Success(_mapper.Map<IEnumerable<PortfolioPicture>>(pictures));
            }
            catch (Exception e)
            {
                return Result.Exception<IEnumerable<PortfolioPicture>>(e);
            }
        }

        public async Task<Result> UpdatePicturesAsync(IEnumerable<PortfolioPicture> updates)
        {
            try
            {
                var portfolioIds = updates.Select(x => x.PortfolioId).Distinct();

                var getQuery = _context.PortfolioPictures.Where(x => portfolioIds.Contains(x.PortfolioId));
                var pictures = await getQuery.ToListAsync();

                foreach (var update in updates)
                {
                    var existingPic = pictures.SingleOrDefault(x => x.Id == update.Id);
                    _mapper.Map(update, existingPic);
                }

                await _context.SaveChangesAsync();

                return Result.Success();
            }
            catch (Exception e)
            {
                return Result.Exception(e);
            }
        }

        public async Task<Result<PortfolioPicture>> GetPictureAsync(int pictureId)
        {
            try
            {
                var portfolioPicture = await _context.PortfolioPictures.SingleOrDefaultAsync(x => x.Id == pictureId);
                if (portfolioPicture == null)
                {
                    return Result.Error<PortfolioPicture>(HttpStatusCode.NotFound);
                }

                return Result.Success(_mapper.Map<PortfolioPicture>(portfolioPicture));
            }
            catch (Exception e)
            {
                return Result.Exception<PortfolioPicture>(e);
            }
        }

        public async Task<Result<Portfolio>> CreateAsync(Portfolio portfolio)
        {
            try
            {
                var addResult = _context.Portfolios.Add(_mapper.Map<Data.Models.Portfolio>(portfolio));
                await _context.SaveChangesAsync(default);

                return Result.Created(_mapper.Map<Portfolio>(addResult.Entity));
            }
            catch (Exception e)
            {
                return Result.Exception<Portfolio>(e);
            }
        }

        public async Task<Result<Portfolio>> UpdateAsync(Portfolio portfolio)
        {
            try
            {
                var existing = await _context.FindAsync<Data.Models.Portfolio>(portfolio.Id);
                _mapper.Map(portfolio, existing);

                await _context.SaveChangesAsync(default);

                return Result.Success(_mapper.Map<Portfolio>(existing));
            }
            catch (Exception e)
            {
                return Result.Exception<Portfolio>(e);
            }
        }

        public async Task<Result> DeleteAsync(int id)
        {
            try
            {
                var portfolio = await _context.FindAsync<Data.Models.Portfolio>(id);
                _context.Remove(portfolio);
                await _context.SaveChangesAsync();

                return Result.Success();
            }
            catch (Exception e)
            {
                return Result.Exception(e);
            }
        }

        public async Task<Result<PortfolioPicture>> CreatePictureAsync(PortfolioPicture picture)
        {
            try
            {
                var addResult = _context.PortfolioPictures.Add(_mapper.Map<Data.Models.PortfolioPicture>(picture));
                await _context.SaveChangesAsync(default);

                return Result.Created(_mapper.Map<PortfolioPicture>(addResult.Entity));
            }
            catch (Exception e)
            {
                return Result.Exception<PortfolioPicture>(e);
            }
        }

        public async Task<Result> DeletePictureAsync(int id)
        {
            try
            {
                var portfolioPicture = await _context.PortfolioPictures.FindAsync(id);
                _context.Remove(portfolioPicture);
                await _context.SaveChangesAsync();

                return Result.Success();
            }
            catch (Exception e)
            {
                return Result.Exception(e);
            }
        }
    }
}
