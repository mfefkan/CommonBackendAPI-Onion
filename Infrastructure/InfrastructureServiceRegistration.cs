using Domain.Interfaces;
using Infrastructure.Database;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore; 

namespace Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string writeConnection, string readConnection)
        {
            // Write DbContext
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(writeConnection)); 

            // Repositories 
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}
