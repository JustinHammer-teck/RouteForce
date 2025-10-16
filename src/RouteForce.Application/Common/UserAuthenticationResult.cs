namespace RouteForce.Application.Common;

public record UserAuthenticationResult(bool Successful, IEnumerable<string> Errors);
