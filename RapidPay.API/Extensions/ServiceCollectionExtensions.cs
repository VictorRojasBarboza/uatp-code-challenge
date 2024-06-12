using RapidPay.Service.Services.Card;
using RapidPay.Service.Services.Token;
using RapidPay.Shared.Utils;

namespace RapidPay.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddScopedServices(this IServiceCollection services)
        {
            // Register all scoped services here
            services.AddScoped<ICardService, CardService>();
            services.AddScoped<ITokenService, TokenService>();

            // Ensure the log directory exists
            var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            // Register FileLogger with the log file path
            services.AddSingleton(new FileLogger(Path.Combine(logDirectory, "logs.txt")));


            // Register any other scoped services as needed

            return services;
        }
    }
}
