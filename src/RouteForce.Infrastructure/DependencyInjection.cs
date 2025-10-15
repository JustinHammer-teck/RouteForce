using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RouteForce.Application.Common.Interfaces;
using RouteForce.Infrastructure.Persistent;

namespace RouteForce.Infrastructure;

public static class DependencyInjection
{
    public static ServiceCollection AddInfrastructure(this ServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>((sp, options) => { options.UseSqlite(connectionString); });

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}