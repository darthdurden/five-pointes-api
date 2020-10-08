using AutoMapper;
using FivePointes.Logic.Models;

namespace FivePointes.Api.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<Data.Models.User, User>().ReverseMap();
        }
    }
}
