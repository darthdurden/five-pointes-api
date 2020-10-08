using FivePointes.Common;
using FivePointes.Logic.Models;
using System.Threading.Tasks;

namespace FivePointes.Logic.Ports
{
    public interface IUsersRepository
    {
        Task<Result<User>> CreateUserAsync(User user);

        Task<Result<User>> GetUserAsync(string username);
    }
}
