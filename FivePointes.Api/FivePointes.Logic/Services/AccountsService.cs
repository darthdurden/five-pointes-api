using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FivePointes.Common;
using FivePointes.Logic.Models;
using FivePointes.Logic.Models.Filters;
using FivePointes.Logic.Ports;

namespace FivePointes.Logic.Services
{
    public class AccountsService : IAccountsService
    {
        private readonly IAccountsRepository _repository;
        private readonly ITransactionsRepository _transactionsRepository;

        public AccountsService(IAccountsRepository repository, ITransactionsRepository transactionsRepository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _transactionsRepository = transactionsRepository ?? throw new ArgumentNullException(nameof(transactionsRepository));
        }

        public Task<Result<Account>> CreateAsync(Account account)
        {
            return _repository.CreateAsync(account);
        }

        public Task<Result> DeleteAsync(long id)
        {
            return _repository.DeleteAsync(id);
        }

        public async Task<Result<Account>> GetAsync(long id)
        {
            var getResult = await _repository.GetAsync(id);

            if (!getResult.IsSuccessful())
            {
                return getResult;
            }

            await PopulateTotalsAsync(getResult.Value);

            return getResult;
        }

        public async Task<Result<IEnumerable<Account>>> GetAsync(AccountFilter filter)
        {
            var accountsResult = await _repository.GetAsync(filter);
            if(accountsResult.IsSuccessful()) 
            {
                var populateTasks = new List<Task>();

                foreach(var account in accountsResult.Value)
                {
                    populateTasks.Add(PopulateTotalsAsync(account));
                }

                await Task.WhenAll(populateTasks);
            }

            return accountsResult;
        }

        public Task<Result<Account>> UpdateAsync(Account account)
        {
            return _repository.UpdateAsync(account);
        }

        private async Task PopulateTotalsAsync(Account account)
        {
            var clearedTask = _transactionsRepository.GetTotalAsync(new TransactionFilter { AccountId = account.Id, IsCleared = true });
            var totalTask = _transactionsRepository.GetTotalAsync(new TransactionFilter { AccountId = account.Id });

            await Task.WhenAll(clearedTask, totalTask);

            if (!clearedTask.Result.IsSuccessful())
            {
                return;
            }

            if (!totalTask.Result.IsSuccessful())
            {
                return;
            }

            account.ClearedTotal = clearedTask.Result.Value;
            account.Total = totalTask.Result.Value;
        }
    }
}
