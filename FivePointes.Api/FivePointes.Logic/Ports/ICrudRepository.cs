using FivePointes.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FivePointes.Logic.Ports
{
    public interface ICrudRepository<TModel, TFilter>
    {
        Task<Result<TModel>> GetAsync(long id);
        Task<Result<IEnumerable<TModel>>> GetAsync(TFilter filter);
        Task<Result<TModel>> UpdateAsync(TModel model);
        Task<Result<TModel>> CreateAsync(TModel model);
        Task<Result> DeleteAsync(long id);
    }
}
