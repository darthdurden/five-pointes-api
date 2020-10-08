using AutoMapper;
using FivePointes.Api.Controllers;
using FivePointes.Api.Controllers.Finances;
using FivePointes.Api.Dtos;
using FivePointes.Api.Dtos.Hal;
using FivePointes.Api.Mappings.Resolvers;
using FivePointes.Logic.Models;
using System.Collections.Generic;

namespace FivePointes.Api.Mappings
{
    public class TransactionCategoryMappingProfile : Profile
    {
        public TransactionCategoryMappingProfile()
        {
            CreateMap<Data.Models.ExpenseCategory, TransactionCategory>()
                .ForMember(m => m.TransactionType, opt => opt.MapFrom(x => x.Type))
                .ReverseMap();

            CreateMap<TransactionCategoryDto, TransactionCategory>().ReverseMap()
                .ForMember(m => m.Links, opt => opt.MapFrom(src => src));

            CreateMap<TransactionCategory, HalLinks>()
                .ForMember(m => m.Self, opt => opt.MapFrom<HalLinkResolver, HalLinkContext>(src => new HalLinkContext
                {
                    ControllerType = typeof(TransactionCategoriesController),
                    MethodName = nameof(TransactionCategoriesController.GetTransactionCategory),
                    PathParameters = new Dictionary<string, string>
                    {
                        { "id", $"{src.Id}" }
                    }
                }));
        }
    }
}
