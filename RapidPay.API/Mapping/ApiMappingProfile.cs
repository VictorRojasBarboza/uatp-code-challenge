using AutoMapper;

namespace RapidPay.API.Mapping
{
    public class ApiMappingProfile : Profile
    {
        public ApiMappingProfile()
        {
            CreateMap<DTO.CreateCardRequest, Service.DTO.CreateCardRequest>();
            CreateMap<Service.DTO.CreateCardResponse, DTO.CreateCardResponse>();
            CreateMap<DAL.Models.Card, Service.DTO.CreateCardResponse>();
        }
    }
}
