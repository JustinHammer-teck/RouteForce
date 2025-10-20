using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RouteForce.Application.Common.Interfaces;
using RouteForce.Application.Common.Settings;
using RouteForce.Infrastructure.External.EmailSevices;
using RouteForce.Infrastructure.Persistent;

namespace RouteForce.Infrastructure;

public static class ServiceConfigurations
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>((sp, options) => { options.UseSqlite(connectionString); });

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));
        services.Configure<WebhookSettings>(configuration.GetSection(WebhookSettings.SectionName));

        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}