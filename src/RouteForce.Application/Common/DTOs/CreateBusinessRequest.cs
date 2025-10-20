namespace RouteForce.Application.Common.DTOs;

public record CreateBusinessRequest(
    string Name,
    string AddressLine,
    string City,
    string State,
    string PostalCode,
    string Country,
    decimal? Latitude,
    decimal? Longitude,
    string Notes);
