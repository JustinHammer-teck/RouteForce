namespace RouteForce.Web.Models;

public record ErrorDetail(int StatusCode, string Title, string Message)
{
    public static ErrorDetail FromStatusCode(int statusCode, string? customMessage = null)
    {
        return statusCode switch
        {
            400 => BadRequest(customMessage),
            401 => Unauthorized(customMessage),
            403 => Forbidden(customMessage),
            404 => NotFound(customMessage),
            405 => MethodNotAllowed(customMessage),
            408 => RequestTimeout(customMessage),
            500 => InternalServerError(customMessage),
            502 => BadGateway(customMessage),
            503 => ServiceUnavailable(customMessage),
            _ => Generic(statusCode, customMessage)
        };
    }

    public static ErrorDetail BadRequest(string? customMessage = null) => new(
        400,
        "Bad Request",
        customMessage ?? "The request could not be understood by the server."
    );

    public static ErrorDetail Unauthorized(string? customMessage = null) => new(
        401,
        "Unauthorized",
        customMessage ?? "You need to authenticate to access this resource."
    );

    public static ErrorDetail Forbidden(string? customMessage = null) => new(
        403,
        "Forbidden",
        customMessage ?? "You don't have permission to access this resource."
    );

    public static ErrorDetail NotFound(string? customMessage = null) => new(
        404,
        "Page Not Found",
        customMessage ?? "The page you're looking for doesn't exist or has been moved."
    );

    public static ErrorDetail MethodNotAllowed(string? customMessage = null) => new(
        405,
        "Method Not Allowed",
        customMessage ?? "The request method is not supported for this resource."
    );

    public static ErrorDetail RequestTimeout(string? customMessage = null) => new(
        408,
        "Request Timeout",
        customMessage ?? "The server timed out waiting for the request."
    );

    public static ErrorDetail InternalServerError(string? customMessage = null) => new(
        500,
        "Internal Server Error",
        customMessage ?? "Something went wrong on our end. We're working to fix it."
    );

    public static ErrorDetail BadGateway(string? customMessage = null) => new(
        502,
        "Bad Gateway",
        customMessage ?? "The server received an invalid response from the upstream server."
    );

    public static ErrorDetail ServiceUnavailable(string? customMessage = null) => new(
        503,
        "Service Unavailable",
        customMessage ?? "The server is currently unavailable. Please try again later."
    );

    public static ErrorDetail Generic(int statusCode, string? customMessage = null) => new(
        statusCode,
        "Error",
        customMessage ?? "An unexpected error occurred."
    );
}
