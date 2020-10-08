using AutoMapper;
using FivePointes.Data;
using FivePointes.Logic.Models;
using FivePointes.Logic.Models.Filters;
using FivePointes.Logic.Ports;

namespace FivePointes.Api.Adapters
{
    public class AccountsRepository : CrudRepository<Account, AccountFilter, Data.Models.Account>, IAccountsRepository
    {
        public AccountsRepository(FivePointesDbContext context, IMapper mapper) : base(mapper, context, context.Accounts) { }
    }
}
