using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using RouteForce.Application.Service;

namespace RouteForce.Application;

public static class ServiceConfigurations
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IUserServices,  UserServices>();
        return services;
    }
}