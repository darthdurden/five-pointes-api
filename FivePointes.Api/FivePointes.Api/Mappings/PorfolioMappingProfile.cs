using AutoMapper;
using FivePointes.Api.Controllers.CSP;
using FivePointes.Api.Dtos;
using FivePointes.Api.Dtos.Hal;
using FivePointes.Api.Mappings.Resolvers;
using FivePointes.Logic.Models;
using System.Collections.Generic;

namespace FivePointes.Api.Mappings
{
    public class PortfolioMappingProfile : Profile
    {
        public PortfolioMappingProfile()
        {
            CreateMap<Data.Models.Portfolio, Portfolio>().ReverseMap();
            CreateMap<Portfolio, PortfolioDto>()
                .ForMember(m => m.Links, opt => opt.MapFrom(src => src));

            CreateMap<PortfolioDto, Portfolio>()
                .IgnoreAllSourcePropertiesWithAnInaccessibleSetter();

            CreateMap<Portfolio, PortfolioHalLinks>()
                .ForMember(m => m.Self, opt => opt.MapFrom<HalLinkResolver, HalLinkContext>(src => new HalLinkContext
                {
                    ControllerType = typeof(PortfoliosController),
                    MethodName = nameof(PortfoliosController.GetPortfolio),
                    PathParameters = new Dictionary<string, string>
                    {
                        { "id", $"{src.Id}" }
                    }
                }))
                .ForMember(m => m.Pictures, opt => opt.MapFrom<HalLinkResolver, HalLinkContext>(src => new HalLinkContext
                {
                    ControllerType = typeof(PortfoliosController),
                    MethodName = nameof(PortfoliosController.GetPortfolioPictures),
                    PathParameters = new Dictionary<string, string>
                    {
                        { "id", $"{src.Id}" }
                    }
                }));

            CreateMap<PictureDto, PortfolioPicture>()
                // As of right now, no properties are updateable, just the SortIndex with is done implicitly
                .ForAllMembers(m => m.Ignore());

            CreateMap<Data.Models.PortfolioPicture, PortfolioPicture>()
              .ForMember(m => m.ThumbnailWidth, opt => opt.MapFrom(src => src.ThumbWidth))
              .ForMember(m => m.ThumbnailHeight, opt => opt.MapFrom(src => src.ThumbHeight))
              .ReverseMap();

            CreateMap<PortfolioPicture, PictureDto>()
                .ForMember(m => m.Links, opt => opt.MapFrom(src => src));

            CreateMap<PortfolioPicture, PictureHalLinks>()
                .ForMember(m => m.Self, opt => opt.MapFrom<HalLinkResolver, HalLinkContext>(src => new HalLinkContext
                {
                    ControllerType = typeof(PortfoliosController),
                    MethodName = nameof(PortfoliosController.GetPortfolioPicture),
                    PathParameters = new Dictionary<string, string>
                    {
                        { "id", $"{src.PortfolioId}" },
                        { "pictureId", $"{src.Id}" }
                    }
                }))
                .ForMember(m => m.Full, opt => opt.MapFrom<HalLinkResolver, HalLinkContext>(src => new HalLinkContext
                {
                    ControllerType = typeof(PortfoliosController),
                    MethodName = nameof(PortfoliosController.GetPortfolioPictureFull),
                    PathParameters = new Dictionary<string, string>
                    {
                        { "id", $"{src.PortfolioId}" },
                        { "pictureId", $"{src.Id}" }
                    }
                }))
                .ForMember(m => m.Thumbnail, opt => opt.MapFrom<HalLinkResolver, HalLinkContext>(src => new HalLinkContext
                {
                    ControllerType = typeof(PortfoliosController),
                    MethodName = nameof(PortfoliosController.GetPortfolioPictureThumbnail),
                    PathParameters = new Dictionary<string, string>
                    {
                        { "id", $"{src.PortfolioId}" },
                        { "pictureId", $"{src.Id}" }
                    }
                }));
        }
    }
}
