using FivePointes.Common;
using FivePointes.Logic.Models;
using FivePointes.Logic.Models.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FivePointes.Logic.Ports
{
    public interface IAccountsService
    {
        Task<Result<Account>> GetAsync(long id);
        Task<Result<IEnumerable<Account>>> GetAsync(AccountFilter filter);
        Task<Result<Account>> UpdateAsync(Account account);
        Task<Result<Account>> CreateAsync(Account account);
        Task<Result> DeleteAsync(long id);
    }
}
