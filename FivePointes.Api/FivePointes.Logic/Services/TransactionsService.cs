using FivePointes.Common;
using FivePointes.Logic.Models;
using FivePointes.Logic.Models.Filters;
using FivePointes.Logic.Ports;
using System;
using System.Collections.Generic;
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

        public Task<Result<IEnumerable<Transaction>>> GetAsync(TransactionFilter filter)
        {
            return _repository.GetAsync(filter);
        }

        public Task<Result<Transaction>> UpdateAsync(Transaction transaction)
        {
            return _repository.UpdateAsync(transaction);
        }
    }
}
