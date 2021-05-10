using FivePointes.Common;
using FivePointes.Logic.Models;
using FivePointes.Logic.Models.Filters;
using System.Threading.Tasks;

namespace FivePointes.Logic.Ports
{
    public interface ITransactionsRepository : ICrudRepository<Transaction, TransactionFilter> 
    {
        Task<Result<Transaction>> GetAsync(string source, string sourceId);

        Task<Result<decimal>> GetTotalAsync(TransactionFilter filter);
    }
}
