using FivePointes.Common;
using FivePointes.Logic.Configuration;
using FivePointes.Logic.Models;
using FivePointes.Logic.Ports;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FivePointes.Logic.Services
{
    internal class PortfoliosService : IPortfoliosService
    {
        private readonly IPortfoliosRepository _repository;
        private readonly IFilesRepository _filesRepository;
        private readonly IImageProcessor _imageProcessor;
        private readonly PortfolioOptions _options;

        public PortfoliosService(IPortfoliosRepository repository, IFilesRepository filesRepository, IImageProcessor imageProcessor, IOptions<PortfolioOptions> options)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _filesRepository = filesRepository ?? throw new ArgumentNullException(nameof(filesRepository));
            _imageProcessor = imageProcessor ?? throw new ArgumentNullException(nameof(imageProcessor));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<Result<PortfolioPicture>> AddPictureAsync(int portfolioId, Stream picture)
        {
            var portfolioResult = await _repository.GetAsync(portfolioId);
            if (!portfolioResult.IsSuccessful())
            {
                return portfolioResult.AsType<PortfolioPicture>();
            }

            var originalImageResult = await _imageProcessor.GetImageAsync(picture);
            if (!originalImageResult.IsSuccessful())
            {
                return originalImageResult.AsType<PortfolioPicture>();
            }

            var thumbnailImageResult = await _imageProcessor.ResizeImageAsync(originalImageResult.Value, _options.ThumbnailMaxWidth ?? originalImageResult.Value.Width, _options.ThumbnailMaxHeight ?? originalImageResult.Value.Height);
            if (!thumbnailImageResult.IsSuccessful())
            {
                return thumbnailImageResult.AsType<PortfolioPicture>();
            }

            var pictureResult = await _repository.CreatePictureAsync(new PortfolioPicture
            {
                PortfolioId = portfolioId,
                Width = originalImageResult.Value.Width,
                Height = originalImageResult.Value.Height,
                ThumbnailWidth = thumbnailImageResult.Value.Width,
                ThumbnailHeight = thumbnailImageResult.Value.Height
            });

            if (!pictureResult.IsSuccessful())
            {
                return pictureResult;
            }

            var thumbnailSaveResult = await _filesRepository.SaveFileAsync(_options.StorageRoot, string.Format(_options.ThumbStorageTemplate, pictureResult.Value.Id), thumbnailImageResult.Value.Data);
            if (!thumbnailSaveResult.IsSuccessful())
            {
                return thumbnailSaveResult.AsType<PortfolioPicture>();
            }

            var fullSaveResult = await _filesRepository.SaveFileAsync(_options.StorageRoot, string.Format(_options.FullStorageTemplate, pictureResult.Value.Id), originalImageResult.Value.Data);
            if (!fullSaveResult.IsSuccessful())
            {
                return fullSaveResult.AsType<PortfolioPicture>();
            }

            return pictureResult;
        }

        public Task<Result<Portfolio>> CreateAsync(Portfolio portfolio)
        {
            return _repository.CreateAsync(portfolio);
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var picturesResult = await _repository.GetPicturesAsync(id);
            if (!picturesResult.IsSuccessful())
            {
                return picturesResult;
            }

            var deleteFilesResult = await _filesRepository.DeleteFilesAsync(_options.StorageRoot, picturesResult.Value.SelectMany(x => new[] {
                string.Format(_options.FullStorageTemplate, x.Id),
                string.Format(_options.ThumbStorageTemplate, x.Id)
            }));

            if (!deleteFilesResult.IsSuccessful())
            {
                return deleteFilesResult;
            }

            return await _repository.DeleteAsync(id);
        }

        public async Task<Result> DeletePictureAsync(int portfolioId, int pictureId)
        {
            var portfolioResult = await _repository.GetAsync(portfolioId);
            if (!portfolioResult.IsSuccessful())
            {
                return portfolioResult;
            }

            var pictureResult = await _repository.GetPictureAsync(pictureId);
            if (!pictureResult.IsSuccessful())
            {
                return pictureResult;
            }

            var results = await Task.WhenAll(
                _repository.DeletePictureAsync(pictureId),
                _filesRepository.DeleteFileAsync(_options.StorageRoot, string.Format(_options.FullStorageTemplate, pictureId)),
                _filesRepository.DeleteFileAsync(_options.StorageRoot, string.Format(_options.ThumbStorageTemplate, pictureId)));

            var failures = results.Where(x => !x.IsSuccessful());
            if (failures.Any())
            {
                return Result.Error(failures.Max(x => x.Status), failures.SelectMany(x => x.Errors));
            }

            return Result.Success();
        }

        public Task<Result<IEnumerable<Portfolio>>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }

        public Task<Result<Portfolio>> GetAsync(int id)
        {
            return _repository.GetAsync(id);
        }

        public async Task<Result<PortfolioPicture>> GetPictureAsync(int portfolioId, int pictureId)
        {
            var portfolioResult = await _repository.GetAsync(portfolioId);
            if (!portfolioResult.IsSuccessful())
            {
                return portfolioResult.AsType<PortfolioPicture>();
            }

            return await _repository.GetPictureAsync(pictureId);
        }

        public async Task<Result<IEnumerable<PortfolioPicture>>> GetPicturesAsync(int id)
        {
            var portfolioResult = await _repository.GetAsync(id);
            if (!portfolioResult.IsSuccessful())
            {
                return portfolioResult.AsType<IEnumerable<PortfolioPicture>>();
            }

            return await _repository.GetPicturesAsync(id);
        }

        public async Task<Result<Stream>> GetPictureStreamAsync(int portfolioId, int pictureId, PictureSize size)
        {
            var portfolioResult = await _repository.GetAsync(portfolioId);
            if (!portfolioResult.IsSuccessful())
            {
                return portfolioResult.AsType<Stream>();
            }

            var pictureResult = await _repository.GetPictureAsync(pictureId);
            if (!pictureResult.IsSuccessful())
            {
                return pictureResult.AsType<Stream>();
            }

            return await _filesRepository.GetFileAsync(_options.StorageRoot, string.Format(size == PictureSize.Full ? _options.FullStorageTemplate : _options.ThumbStorageTemplate, pictureResult.Value.Id));
        }

        public async Task<Result<Portfolio>> UpdateAsync(Portfolio portfolio)
        {
            var portfolioResult = await _repository.GetAsync(portfolio.Id);
            if (!portfolioResult.IsSuccessful())
            {
                return portfolioResult.AsType<Portfolio>();
            }

            return await _repository.UpdateAsync(portfolio);
        }

        public async Task<Result<IEnumerable<PortfolioPicture>>> UpdatePicturesAsync(int id, IEnumerable<PortfolioPicture> pictures)
        {
            var portfolioResult = await _repository.GetAsync(id);
            if (!portfolioResult.IsSuccessful())
            {
                return portfolioResult.AsType<IEnumerable<PortfolioPicture>>();
            }

            var updateResult = await _repository.UpdatePicturesAsync(pictures.Where(x => x.PortfolioId == id));
            if (!updateResult.IsSuccessful())
            {
                return updateResult.AsType<IEnumerable<PortfolioPicture>>();
            }

            return await _repository.GetPicturesAsync(id);
        }
    }

}
