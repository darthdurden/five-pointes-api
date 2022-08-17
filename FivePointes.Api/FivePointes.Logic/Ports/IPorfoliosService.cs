using FivePointes.Common;
using FivePointes.Logic.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FivePointes.Logic.Ports
{
    public interface IPortfoliosService
    {
        Task<Result<IEnumerable<Portfolio>>> GetAllAsync();
        Task<Result<Portfolio>> CreateAsync(Portfolio portfolio);
        Task<Result<Portfolio>> UpdateAsync(Portfolio portfolio);
        Task<Result<Portfolio>> GetAsync(int id);
        Task<Result> DeleteAsync(int id);
        Task<Result<IEnumerable<PortfolioPicture>>> GetPicturesAsync(int id);
        Task<Result<IEnumerable<PortfolioPicture>>> UpdatePicturesAsync(int id, IEnumerable<PortfolioPicture> pictures);
        Task<Result<PortfolioPicture>> GetPictureAsync(int portfolioId, int pictureId);
        Task<Result<PortfolioPicture>> AddPictureAsync(int portfolioId, Stream picture);
        Task<Result> DeletePictureAsync(int portfolioId, int pictureId);
        Task<Result<Stream>> GetPictureStreamAsync(int portfolioId, int pictureId, PictureSize size);
    }
}
