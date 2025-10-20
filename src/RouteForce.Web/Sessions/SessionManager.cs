namespace RouteForce.Web.Sessions;

public class SessionManager
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SessionManager(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void SetSessionValue(string key, string value)
    {
        _httpContextAccessor.HttpContext?.Session.SetString(key, value);
    }

    public string? GetSessionValue(string key)
    {
        if (_httpContextAccessor.HttpContext == null) return "";

        var message = _httpContextAccessor.HttpContext.Session.GetString(key);
        _httpContextAccessor.HttpContext.Session.Remove(key);
        return message;
    }

    public bool HasKey(string key)
    {
        if (_httpContextAccessor.HttpContext == null) return false;

        var message = _httpContextAccessor.HttpContext.Session.GetString(key);
        return !string.IsNullOrEmpty(message);
    }
}