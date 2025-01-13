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
            services.AddDbContext<AuthDBContext>(options => options.UseNpgsql(writeConnection));

            // Read DbContext
            services.AddDbContext<ReadOnlyAuthDBContext>(options => options.UseNpgsql(readConnection));

            // Repositories
            services.AddScoped<IUserReadRepository, UserReadRepository>();
            services.AddScoped<IUserWriteRepository, UserWriteRepository>();

            return services;
        }
    }
}
