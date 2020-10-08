using FivePointes.Common;
using FivePointes.Logic.Models;
using FivePointes.Logic.Models.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FivePointes.Logic.Ports
{
    public interface ITransactionsService
    {
        Task<Result<Transaction>> GetAsync(long id);
        Task<Result<IEnumerable<Transaction>>> GetAsync(TransactionFilter filter);
        Task<Result<Transaction>> UpdateAsync(Transaction transaction);
        Task<Result<Transaction>> CreateAsync(Transaction transaction);
        Task<Result> DeleteAsync(long id);
    }
}
