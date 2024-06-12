using AutoMapper;

namespace RapidPay.Service.Mapping
{
    public class ServiceMappingProfile : Profile
    {
        public ServiceMappingProfile()
        {
            CreateMap<DAL.Models.Card, Service.DTO.CreateCardResponse>();
        }
    }
}
