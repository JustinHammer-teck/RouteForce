using RouteForce.Core.Enums;

namespace RouteForce.Application.Common.DTOs;

public record GetOrdersRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public int GetValidPageSize() => PageSize > 100 ? 100 : PageSize < 1 ? 10 : PageSize;
    public int GetValidPageNumber() => PageNumber < 1 ? 1 : PageNumber;
}
