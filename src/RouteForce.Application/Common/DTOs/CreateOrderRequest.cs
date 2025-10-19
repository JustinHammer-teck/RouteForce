namespace RouteForce.Application.Common.DTOs;

public record CreateOrderRequest(
    int PersonalReceiverId,
    string ProductReferenceId,
    int? SelectedDeliveryAddressId,
    string? Notes,
    DateTime? EstimatedDeliveryDate);