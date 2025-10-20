namespace RouteForce.Application.Common.DTOs;

public class DashboardStatsDto
{
    public int TotalOrders { get; set; }
    public int CreatedCount { get; set; }
    public int InTransitCount { get; set; }
    public int DeliveredCount { get; set; }
}
