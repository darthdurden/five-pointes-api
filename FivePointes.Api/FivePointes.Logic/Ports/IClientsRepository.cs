using FivePointes.Common;
using FivePointes.Logic.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FivePointes.Logic.Ports
{
    public interface IClientsRepository
    {
        Task<Result<IEnumerable<Client>>> GetAllAsync();
        Task<Result<Client>> UpdateAsync(Client client);
    }
}
