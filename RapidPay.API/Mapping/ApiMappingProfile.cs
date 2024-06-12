using AutoMapper;

namespace RapidPay.API.Mapping
{
    public class ApiMappingProfile : Profile
    {
        public ApiMappingProfile()
        {
            CreateMap<DTO.CreateCardRequest, Service.DTO.CreateCardRequest>();
            CreateMap<DTO.PaymentRequest, Service.DTO.PaymentRequest>();

            CreateMap<Service.DTO.CreateCardResponse, DTO.CreateCardResponse>();
            CreateMap<Service.DTO.PaymentResponse, DTO.PaymentResponse>();
            
            CreateMap<DAL.Models.Card, Service.DTO.CreateCardResponse>();
            CreateMap<DAL.Models.Payment, Service.DTO.PaymentResponse>();


        }
    }
}
