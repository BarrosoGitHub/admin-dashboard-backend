using OPTConfigurator.Services;

namespace OPTConfigurator.Middleware;

public class RebootingStateMiddleware
{
    private readonly RequestDelegate _next;

    public RebootingStateMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/info/services/isalive", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        // If system is rebooting, block all other endpoints
        if (ApplicationState.IsRebooting)
        {
            context.Response.StatusCode = 503;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                status = "rebooting",
                message = "System is rebooting. All services are temporarily unavailable."
            });
            return;
        }

        await _next(context);
    }
}
