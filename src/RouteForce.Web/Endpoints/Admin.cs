using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RouteForce.Application.Common.DTOs;
using RouteForce.Application.Common.Interfaces;
using RouteForce.Core.Enums;
using RouteForce.Web.Configurations;
using RouteForce.Web.Pages.Admin;

namespace RouteForce.Web.Endpoints;

public class Admin : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization(opt => 
            opt.RequireRole("Admin"));
        
        groupBuilder.MapGet("dashboard", Dashboard).AllowAnonymous();
        groupBuilder.MapGet("stats", GetStats);
    }

    public async Task<RazorComponentResult> Dashboard()
    {
        return new RazorComponentResult<Dashboard>();
    }
    
    
    private async Task<IResult> GetStats(
        IApplicationDbContext context,
        HttpContext httpContext)
    {
        var businessIdClaim = httpContext.User.FindFirst("BusinessId");

        if (businessIdClaim == null || !int.TryParse(businessIdClaim.Value, out var businessId))
        {
            return Results.Unauthorized();
        }

        var orders = await context.Orders
            .AsNoTracking()
            .Where(o => o.BusinessId == businessId)
            .ToListAsync()
            .ConfigureAwait(false);

        var stats = new DashboardStatsDto
        {
            TotalOrders = orders.Count,
            CreatedCount = orders.Count(o => o.Status == OrderStatus.Created),
            InTransitCount = orders.Count(o => o.Status == OrderStatus.InTransit),
            DeliveredCount = orders.Count(o => o.Status == OrderStatus.Delivered)
        };

        return new RazorComponentResult<_DashboardStats>(new
        {
            Stats = stats
        });
    }}