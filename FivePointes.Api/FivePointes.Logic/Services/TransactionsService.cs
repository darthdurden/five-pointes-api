using FivePointes.Common;
using FivePointes.Logic.Models;
using FivePointes.Logic.Models.Filters;
using FivePointes.Logic.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FivePointes.Logic.Services
{
    public class TransactionsService : ITransactionsService
    {
        private readonly ITransactionsRepository _repository;
        private readonly ITransactionCategoriesService _transactionCategoriesService;
        private readonly IAccountsService _accountsService;

        public TransactionsService(ITransactionsRepository repository, ITransactionCategoriesService transactionCategoriesService, IAccountsService accountsService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _transactionCategoriesService = transactionCategoriesService ?? throw new ArgumentNullException(nameof(transactionCategoriesService));
            _accountsService = accountsService ?? throw new ArgumentNullException(nameof(accountsService));
        }

        public async Task<Result<Transaction>> CreateAsync(Transaction transaction)
        {
            if(transaction.Account != null)
            {
                var accountResult = await _accountsService.GetAsync(transaction.Account.Id);
                if(!accountResult.IsSuccessful())
                {
                    return Result.Error<Transaction>(HttpStatusCode.BadRequest, $"Account with id {transaction.Account.Id} does not exist.");
                }
            }

            if (transaction.Category != null)
            {
                var categoryResult = await _transactionCategoriesService.GetAsync(transaction.Category.Id);
                if (!categoryResult.IsSuccessful())
                {
                    return Result.Error<Transaction>(HttpStatusCode.BadRequest, $"Category with id {transaction.Category.Id} does not exist.");
                }
            }
            else if(transaction.Account == null)
            {
                return Result.Error<Transaction>(HttpStatusCode.BadRequest, "Cannot create a transaction without a category or account.");
            }

            if(transaction.Account == null)
            {
                transaction.IsCleared = true;
            }

            return await _repository.CreateAsync(transaction);
        }

        public Task<Result> DeleteAsync(long id)
        {
            return _repository.DeleteAsync(id);
        }

        public Task<Result<Transaction>> GetAsync(long id)
        {
            return _repository.GetAsync(id);
        }

        public Task<Result<Transaction>> GetAsync(string source, string sourceId)
        {
            return _repository.GetAsync(source, sourceId);
        }

        public async Task<Result<IEnumerable<Transaction>>> GetAsync(TransactionFilter filter)
        {
            // If filtering by cleared, just filter as specified
            if(filter.IsCleared.HasValue)
            {
                var transResult = await _repository.GetAsync(filter);
                if(!transResult.IsSuccessful())
                {
                    return transResult;
                }

                return Result.Success(SortTransactions(transResult.Value));
            }

            // If not filtering by cleared, grab all the cleared transactions based on the filter
            filter.IsCleared = true;
            var cleared = await _repository.GetAsync(filter);

            if(!cleared.IsSuccessful())
            {
                return cleared;
            }

            // Then grab all of the uncleared transactions all time
            filter.IsCleared = false;
            filter.StartDate = null;
            filter.EndDate = null;
            var uncleared = await _repository.GetAsync(filter);

            if(!uncleared.IsSuccessful())
            {
                return uncleared;
            }

            return Result.Success(SortTransactions(uncleared.Value.Concat(cleared.Value)));
        }

        private IEnumerable<Transaction> SortTransactions(IEnumerable<Transaction> unsorted)
        {
            return unsorted.OrderBy(x => x.IsCleared).ThenByDescending(x => x.Date);
        }

        public async Task<Result<Transaction>> UpdateAsync(Transaction transaction)
        {
            if(transaction.Account == null)
            {
                transaction.IsCleared = true;
            }

            var existingResult = await _repository.GetAsync(transaction.Id);
            if (!existingResult.IsSuccessful())
            {
                return existingResult;
            }

            transaction.Source = existingResult.Value.Source;
            transaction.SourceId = existingResult.Value.SourceId;

            return await _repository.UpdateAsync(transaction);
        }
    }
}
