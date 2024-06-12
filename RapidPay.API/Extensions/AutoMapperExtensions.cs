using RapidPay.API.Mapping;

namespace RapidPay.API.Extensions
{
    public static class AutoMapperExtensions
    {
        public static IServiceCollection AddCustomAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ApiMappingProfile));
            return services;
        }
    }
}
