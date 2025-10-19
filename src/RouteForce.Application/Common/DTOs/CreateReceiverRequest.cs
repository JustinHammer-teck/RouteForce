namespace RouteForce.Application.Common.DTOs;

public record CreateReceiverRequest(
    string Name,
    string Email,
    string Phone,
    string? Notes,
    string AddressLine,
    string City,
    string State,
    string PostalCode,
    string Country,
    decimal? Latitude,
    decimal? Longitude,
    string? AddressLabel);