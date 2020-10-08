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
    public class TransactionMappingProfile : Profile
    {
        public TransactionMappingProfile()
        {
            CreateMap<Data.Models.Expense, Transaction>()
                .ForMember(x => x.Amount, opt => opt.MapFrom(x => x.Price))
                .ForMember(x => x.Date, opt => opt.MapFrom(x => x.DatePaid))
                .ReverseMap()
                .ForMember(x => x.Account, opt => opt.Ignore())
                .ForMember(x => x.AccountId, opt => opt.MapFrom(x => x.Account != null ? x.Account.Id : (long?)null))
                .ForMember(x => x.Category, opt => opt.Ignore())
                .ForMember(x => x.CategoryId, opt => opt.MapFrom(x => x.Category != null ? x.Category.Id : (long?)null));

            CreateMap<TransactionDto, Transaction>().ReverseMap()
                .ForMember(m => m.Type, opt => opt.MapFrom(src => src.Category != null ? src.Category.TransactionType : (TransactionType?)null))
                .ForMember(m => m.Links, opt => opt.MapFrom(src => src));

            CreateMap<Transaction, TransactionHalLinks>()
                .ForMember(m => m.Self, opt => opt.MapFrom<HalLinkResolver, HalLinkContext>(src => new HalLinkContext
                {
                    ControllerType = typeof(TransactionsController),
                    MethodName = nameof(TransactionsController.GetTransaction),
                    PathParameters = new Dictionary<string, string>
                    {
                        { "id", $"{src.Id}" }
                    }
                }))
                .ForMember(m => m.Account, opt => opt.MapFrom<HalLinkResolver, HalLinkContext>(src => src.Account != null ? new HalLinkContext
                {
                    ControllerType = typeof(AccountsController),
                    MethodName = nameof(AccountsController.GetAccount),
                    PathParameters = new Dictionary<string, string>
                    {
                        { "id", $"{src.Account.Id}" }
                    }
                } : null))
                .ForMember(m => m.Category, opt => opt.MapFrom<HalLinkResolver, HalLinkContext>(src => src.Category != null ? new HalLinkContext
                {
                    ControllerType = typeof(TransactionCategoriesController),
                    MethodName = nameof(TransactionCategoriesController.GetTransactionCategory),
                    PathParameters = new Dictionary<string, string>
                    {
                        { "id", $"{src.Category.Id}" }
                    }
                } : null));
        }
    }
}
