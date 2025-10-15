using Htmx;

namespace RouteForce.Web.Sessions;

public class HtmxRequestContext
{
    private readonly IHttpContextAccessor _context;

    public HtmxRequestContext(IHttpContextAccessor context)
    {
        _context = context;
    }

    public bool IsHtmx()
    {
        var htmx = _context.HttpContext.Request.IsHtmx();
        var boost = _context.HttpContext.Request.IsHtmxBoosted();

        return htmx && !boost;
    }
}