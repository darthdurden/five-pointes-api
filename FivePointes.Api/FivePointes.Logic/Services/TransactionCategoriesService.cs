using FivePointes.Common;
using FivePointes.Logic.Models;
using FivePointes.Logic.Models.Filters;
using FivePointes.Logic.Ports;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FivePointes.Logic.Services
{
    public class TransactionCategoriesService : ITransactionCategoriesService
    {
        private readonly ITransactionCategoriesRepository _repository;

        public TransactionCategoriesService(ITransactionCategoriesRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Task<Result<TransactionCategory>> CreateAsync(TransactionCategory transactionCategory)
        {
            return _repository.CreateAsync(transactionCategory);
        }

        public Task<Result> DeleteAsync(long id)
        {
            return _repository.DeleteAsync(id);
        }

        public Task<Result<TransactionCategory>> GetAsync(long id)
        {
            return _repository.GetAsync(id);
        }

        public Task<Result<IEnumerable<TransactionCategory>>> GetAsync(TransactionCategoryFilter filter)
        {
            return _repository.GetAsync(filter);
        }

        public Task<Result<TransactionCategory>> UpdateAsync(TransactionCategory transactionCategory)
        {
            return _repository.UpdateAsync(transactionCategory);
        }
    }
}
