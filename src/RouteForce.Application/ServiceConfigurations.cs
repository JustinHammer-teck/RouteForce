using Microsoft.Extensions.DependencyInjection;
using RouteForce.Application.Common.Interfaces;
using RouteForce.Application.Service.Order;
using RouteForce.Application.Service.RouteCheckpoint;
using RouteForce.Application.Service.WebhookToken;

namespace RouteForce.Application;

public static class ServiceConfigurations
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        
        services.AddTransient<IWebhookService, WebhookService>();
        services.AddTransient<IOrderService, OrderService>();
        services.AddTransient<IRouteCheckpointService, RouteCheckpointService>();
        
        return services;
    }
}