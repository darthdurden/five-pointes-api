using AutoMapper;
using FivePointes.Data;
using FivePointes.Logic.Models;
using FivePointes.Logic.Models.Filters;
using FivePointes.Logic.Ports;

namespace FivePointes.Api.Adapters
{
    public class TransactionCategoriesRepository : CrudRepository<TransactionCategory, TransactionCategoryFilter, Data.Models.ExpenseCategory>, ITransactionCategoriesRepository
    {
        public TransactionCategoriesRepository(FivePointesDbContext context, IMapper mapper) : base(mapper, context, context.ExpenseCategories) { }
    }
}
