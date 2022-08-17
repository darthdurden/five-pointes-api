using FivePointes.Common;
using FivePointes.Logic.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FivePointes.Logic.Ports
{
    public interface IPortfoliosRepository
    {
        Task<Result<IEnumerable<Portfolio>>> GetAllAsync();
        Task<Result<Portfolio>> CreateAsync(Portfolio portfolio);
        Task<Result<Portfolio>> UpdateAsync(Portfolio portfolio);
        Task<Result<Portfolio>> GetAsync(int id);
        Task<Result> DeleteAsync(int id);
        Task<Result<IEnumerable<PortfolioPicture>>> GetPicturesAsync(int id);
        Task<Result> UpdatePicturesAsync(IEnumerable<PortfolioPicture> updates);
        Task<Result<PortfolioPicture>> GetPictureAsync(int pictureId);
        Task<Result<PortfolioPicture>> CreatePictureAsync(PortfolioPicture picture);
        Task<Result> DeletePictureAsync(int pictureId);
    }
}
