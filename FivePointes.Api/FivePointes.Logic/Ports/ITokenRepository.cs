using FivePointes.Common;
using FivePointes.Logic.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FivePointes.Logic.Ports
{
    public interface ITokenRepository
    {
        Task<Result<Token>> GenerateTokenAsync(ClaimsIdentity identity);
    }
}
