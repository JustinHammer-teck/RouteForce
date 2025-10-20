namespace RouteForce.Application.Common.DTOs;

public record CreateOrderRequest(
    int PersonalReceiverId,
    string ProductReferenceId,
    int? SelectedDeliveryAddressId,
    string? IntermediateCheckpointName,
    string? AddressLine,
    string? City,
    string? State,
    string? PostalCode,
    string? Country,
    decimal? Latitude,
    decimal? Longitude,
    string? Notes,
    DateTime? EstimatedDeliveryDate);