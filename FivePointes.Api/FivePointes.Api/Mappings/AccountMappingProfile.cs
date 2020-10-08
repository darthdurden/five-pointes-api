using AutoMapper;
using FivePointes.Api.Controllers;
using FivePointes.Api.Controllers.Finances;
using FivePointes.Api.Dtos;
using FivePointes.Api.Dtos.Hal;
using FivePointes.Api.Mappings.Resolvers;
using FivePointes.Logic.Models;
using System;
using System.Collections.Generic;

namespace FivePointes.Api.Mappings
{
    public class AccountMappingProfile : Profile
    {
        public AccountMappingProfile()
        {
            CreateMap<Data.Models.Account, Account>()
                .ReverseMap();

            CreateMap<AccountDto, Account>().ReverseMap()
                .ForMember(m => m.Links, opt => opt.MapFrom(src => src));

            CreateMap<Account, AccountHalLinks>()
                .ForMember(m => m.Self, opt => opt.MapFrom<HalLinkResolver, HalLinkContext>(src => new HalLinkContext
                {
                    ControllerType = typeof(AccountsController),
                    MethodName = nameof(AccountsController.GetAccount),
                    PathParameters = new Dictionary<string, string>
                    {
                        { "id", $"{src.Id}" }
                    }
                }))
                .ForMember(m => m.RecentTransactions, opt => opt.MapFrom<HalLinkResolver, HalLinkContext>(src => new HalLinkContext
                {
                    ControllerType = typeof(TransactionsController),
                    MethodName = nameof(TransactionsController.GetTransactions),
                    QueryParameters = new Dictionary<string, string>
                    {
                        { "accountId", $"{src.Id}" },
                        { "startDate", DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd") }
                    }
                }))
                .ForMember(m => m.UnclearedTransactions, opt => opt.MapFrom<HalLinkResolver, HalLinkContext>(src => new HalLinkContext
                {
                    ControllerType = typeof(TransactionsController),
                    MethodName = nameof(TransactionsController.GetTransactions),
                    QueryParameters = new Dictionary<string, string>
                    {
                        { "accountId", $"{src.Id}" },
                        { "isCleared", "false" }
                    }
                }));
        }
    }
}
