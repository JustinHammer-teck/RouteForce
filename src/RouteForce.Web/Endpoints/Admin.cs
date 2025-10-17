using Microsoft.AspNetCore.Http.HttpResults;
using RouteForce.Web.Configurations;
using RouteForce.Web.Pages.Admin;

namespace RouteForce.Web.Endpoints;

public class Admin : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization(opt => 
            opt.RequireRole("Admin"));
        
        /*
        groupBuilder.MapGet("dashboard", Dashboard)
            .RequireAuthorization(opt =>
            {
                opt.RequireRole("AppUser");
            });
            */
        
        groupBuilder.MapGet("dashboard", Dashboard).AllowAnonymous();
    }

    public async Task<RazorComponentResult> Dashboard()
    {
        return new RazorComponentResult<Dashboard>();
    }
}