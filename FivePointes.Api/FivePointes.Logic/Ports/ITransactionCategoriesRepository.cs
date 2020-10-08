using FivePointes.Logic.Models;
using FivePointes.Logic.Models.Filters;

namespace FivePointes.Logic.Ports
{
    public interface ITransactionCategoriesRepository : ICrudRepository<TransactionCategory, TransactionCategoryFilter> { }
}
