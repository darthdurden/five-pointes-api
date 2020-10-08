using FivePointes.Common;
using FivePointes.Logic.Models;
using System.Threading.Tasks;

namespace FivePointes.Logic.Ports
{
    public interface IUsersService
    {
        Task<Result<User>> CreateUserAsync(string username, string password);

        Task<Result<Token>> LoginUserAsync(string username, string password);
    }
}
