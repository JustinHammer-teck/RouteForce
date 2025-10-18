using Microsoft.Extensions.DependencyInjection;
using RouteForce.Application.Common.Interfaces;

namespace RouteForce.Application;

public static class ServiceConfigurations
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}