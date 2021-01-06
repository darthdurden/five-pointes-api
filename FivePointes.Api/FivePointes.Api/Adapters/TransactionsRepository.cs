using AutoMapper;
using FivePointes.Common;
using FivePointes.Data;
using FivePointes.Logic.Models;
using FivePointes.Logic.Models.Filters;
using FivePointes.Logic.Ports;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FivePointes.Api.Adapters
{
    public class TransactionsRepository : CrudRepository<Transaction, TransactionFilter, Data.Models.Expense>, ITransactionsRepository
    {
        public TransactionsRepository(FivePointesDbContext context, IMapper mapper) : base(mapper, context, context.Expenses, new[] { "Category", "Account" }) { }

        public async Task<Result<decimal>> GetTotalAsync(TransactionFilter filter)
        {
            try
            {
                var query = _models.AsQueryable();

                query = FilterQuery(query, filter);

                var result = await query.SumAsync(x => x.Price);

                return Result.Success(result);
            }
            catch (Exception e)
            {
                return Result.Exception<decimal>(e);
            }
        }

        protected override IQueryable<Data.Models.Expense> FilterQuery(IQueryable<Data.Models.Expense> query, TransactionFilter filter)
        {
            query = base.FilterQuery(query, filter);

            if (filter.StartDate.HasValue)
            {
                var startDate = filter.StartDate.Value.ToDateTimeUnspecified();
                query = query.Where(x => x.DatePaid >= startDate);
            }

            if (filter.EndDate.HasValue)
            {
                var endDate = filter.EndDate.Value.ToDateTimeUnspecified();
                query = query.Where(x => x.DatePaid <= endDate);
            }

            if (filter.IsCleared.HasValue)
            {
                query = query.Where(x => x.IsCleared == filter.IsCleared.Value);
            }

            if (filter.AccountId.HasValue)
            {
                query = query.Where(x => x.AccountId == filter.AccountId.Value);
            }

            if (filter.CategoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == filter.CategoryId.Value);
            }

            if(filter.Types != null)
            {
                query = query.Where(x => filter.Types.Cast<int>().Contains(x.Category.Type));
            }

            return query.OrderBy(x => x.IsCleared).ThenByDescending(x => x.DatePaid);
        }
    }
}
