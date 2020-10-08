using AutoMapper;
using FivePointes.Api.Dtos;
using FivePointes.Logic.Models;
using NodaTime;

namespace FivePointes.Api.Mappings
{
    public class ClientMappingProfile : Profile
    {
        public ClientMappingProfile()
        {
            CreateMap<Clockify.Net.Models.Clients.ClientDto, Client>();

            CreateMap<Client, ClientDto>()
                .ForMember(m => m.CommittedHours, opt => opt.MapFrom(x => x.Commitment.TotalHours));

            CreateMap<ClientDto, Client>()
                .ForMember(m => m.Commitment, opt => opt.MapFrom(x => Duration.FromHours(x.CommittedHours)));
        }
    }
}
