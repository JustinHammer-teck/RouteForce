using Htmx;
using Microsoft.AspNetCore.Authentication.Cookies;
using RouteForce.Web.Sessions;

namespace RouteForce.Web;

public static class ServiceConfigurations
{
    public static IServiceCollection AddWebApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRazorPages();
        services.AddHttpContextAccessor();
        services.AddSession(options => {
            options.Cookie.Name = ".register-user.form";
            options.IdleTimeout = TimeSpan.FromMinutes(5);
        });
        services.AddTransient<SessionManager>();    
        services.AddTransient<HtmxRequestContext>();
        
        return services;
    }

    public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "AuthenticationCookie";
                options.LoginPath = "/users/login";
                options.AccessDeniedPath = "/error/403?msg='Access denied'";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

        services.AddAuthorization();
        services.AddAntiforgery();
        services.AddCors(options =>
        {
            options.AddPolicy("htmxcorspolicy",
                policy =>
                {
                    policy.WithOrigins("*")
                        .WithHeaders(HtmxRequestHeaders.Keys.All)
                        .WithExposedHeaders(HtmxResponseHeaders.Keys.All);
                });
        });
        
        

        return services;
    }
    
}