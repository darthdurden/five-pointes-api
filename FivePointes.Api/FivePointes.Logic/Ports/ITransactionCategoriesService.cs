using FivePointes.Common;
using FivePointes.Logic.Models;
using FivePointes.Logic.Models.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FivePointes.Logic.Ports
{
    public interface ITransactionCategoriesService
    {
        Task<Result<TransactionCategory>> GetAsync(long id);
        Task<Result<IEnumerable<TransactionCategory>>> GetAsync(TransactionCategoryFilter filter);
        Task<Result<TransactionCategory>> UpdateAsync(TransactionCategory transactionCategory);
        Task<Result<TransactionCategory>> CreateAsync(TransactionCategory transactionCategory);
        Task<Result> DeleteAsync(long id);
    }
}
