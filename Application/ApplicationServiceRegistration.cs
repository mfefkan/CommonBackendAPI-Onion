using Application.Services;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection; 


namespace Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, string writeConnection, string readConnection)
        {
            // Infrastructure bağımlılıklarını kaydet
            services.AddInfrastructureServices(writeConnection, readConnection);

            // Application içindeki servisleri kaydet
            services.AddScoped<AuthService>();

            return services;
        }
    }
}
