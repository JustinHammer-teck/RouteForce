using RouteForce.Core.Enums;

namespace RouteForce.Application.Common.DTOs;

public record CreateUserRequest(
    string UserName,
    string Password,
    string Email,
    string Phone,
    UserRole UserRole);
