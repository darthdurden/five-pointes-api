using AutoMapper;
using FivePointes.Common;
using FivePointes.Data.Models;
using FivePointes.Logic.Models;
using FivePointes.Logic.Ports;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FivePointes.Api.Adapters
{
    public abstract class CrudRepository<TModel, TFilter, TDataModel> : ICrudRepository<TModel, TFilter>
        where TModel : IModel
        where TDataModel : class, IDataModel
    {
        protected readonly IMapper _mapper;
        protected readonly DbContext _context;
        protected readonly DbSet<TDataModel> _models;
        private readonly IEnumerable<string> _navigationPropertiesToInclude;

        public CrudRepository(IMapper mapper, DbContext context, DbSet<TDataModel> models, IEnumerable<string> navigationPropertiesToInclude = null)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _models = models ?? throw new ArgumentNullException(nameof(models));
            _navigationPropertiesToInclude = navigationPropertiesToInclude;
        }

        public async Task<Result<TModel>> CreateAsync(TModel model)
        {
            try
            {
                var addResult = _models.Add(_mapper.Map<TDataModel>(model));
                await _context.SaveChangesAsync(default);

                var created = _mapper.Map<TModel>(addResult.Entity);

                // Fetch it if we are grabbing naviation properties
                if(_navigationPropertiesToInclude?.Any() == true)
                {
                    var getResult = await GetAsync(created.Id);
                    if (getResult.IsSuccessful())
                    {
                        return Result.Created(getResult.Value);
                    }
                }

                return Result.Created(created);
            }
            catch (Exception e)
            {
                return Result.Exception<TModel>(e);
            }
        }

        public async Task<Result> DeleteAsync(long id)
        {
            try
            {
                var dataModel = await _context.FindAsync<TDataModel>(id);
                _context.Remove(dataModel);
                await _context.SaveChangesAsync();

                return Result.Success();
            }
            catch (Exception e)
            {
                return Result.Exception(e);
            }
        }

        public async Task<Result<TModel>> GetAsync(long id)
        {
            try
            {
                TDataModel dataModel = null;
                var query = _models.AsQueryable();

                if (_navigationPropertiesToInclude?.Any() == true)
                {
                    foreach (var prop in _navigationPropertiesToInclude)
                    {
                        query = query.Include(prop);
                    }

                    dataModel = await query.SingleOrDefaultAsync(x => x.Id == id);
                }
                else
                {
                    dataModel = await _models.FindAsync(id);
                }

                if (dataModel == null)
                {
                    return Result.Error<TModel>(HttpStatusCode.NotFound);
                }
                // TODO include navigation properties

                return Result.Success(_mapper.Map<TModel>(dataModel));
            }
            catch (Exception e)
            {
                return Result.Exception<TModel>(e);
            }
        }

        protected virtual IQueryable<TDataModel> FilterQuery(IQueryable<TDataModel> query, TFilter filter)
        {
            return query;
        }

        public async Task<Result<IEnumerable<TModel>>> GetAsync(TFilter filter)
        {
            try
            {
                var query = _models.AsQueryable();

                if(_navigationPropertiesToInclude?.Any() == true)
                {
                    foreach(var prop in _navigationPropertiesToInclude)
                    {
                        query = query.Include(prop);
                    }
                }

                query = FilterQuery(query, filter);

                var dataModels = await query.ToListAsync();

                return Result.Success(_mapper.Map<IEnumerable<TModel>>(dataModels));
            }
            catch (Exception e)
            {
                return Result.Exception<IEnumerable<TModel>>(e);
            }
        }

        public async Task<Result<TModel>> UpdateAsync(TModel model)
        {
            try
            {
                // We don't use the get call here so that we can preserve entity tracking
                var existing = await _context.FindAsync<TDataModel>(model.Id);
                _mapper.Map(model, existing);

                await _context.SaveChangesAsync(default);

                var updated = _mapper.Map<TModel>(existing);

                // Fetch it if we are grabbing naviation properties
                if (_navigationPropertiesToInclude?.Any() == true)
                {
                    var getResult = await GetAsync(updated.Id);
                    if (getResult.IsSuccessful())
                    {
                        return getResult;
                    }
                }

                return Result.Success(updated);
            }
            catch (Exception e)
            {
                return Result.Exception<TModel>(e);
            }
        }
    }
}
