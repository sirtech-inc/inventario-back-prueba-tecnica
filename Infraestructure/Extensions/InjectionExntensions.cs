using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infraestructure.Extensions
{
    public static class InjectionExntensions
    {
        public static IServiceCollection AddInjectionInfraestructure(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = typeof(InventarioContext).Assembly.FullName;

            services.AddDbContext<InventarioContext>(
                options => options.UseSqlServer(
                    configuration.GetConnectionString("InventarioConnection"), b => b.MigrationsAssembly(assembly)), ServiceLifetime.Scoped
                
            );

            return services;
        }
    }
}
