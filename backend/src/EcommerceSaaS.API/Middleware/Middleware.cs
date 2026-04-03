namespace EcommerceSaaS.API.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantMiddleware> _logger;

    public TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var tenantId = ResolveTenantId(context);

        if (tenantId != Guid.Empty)
        {
            context.Items["TenantId"] = tenantId;
            _logger.LogInformation("Tenant resolved: {TenantId}", tenantId);
        }

        await _next(context);
    }

    private static Guid ResolveTenantId(HttpContext context)
    {
        // Try to get from header
        if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantIdHeader))
        {
            if (Guid.TryParse(tenantIdHeader.ToString(), out var tenantId))
                return tenantId;
        }

        // Try to get from subdomain
        var host = context.Request.Host.Host;
        var subdomain = host.Split('.').FirstOrDefault();

        if (!string.IsNullOrEmpty(subdomain) && subdomain != "localhost")
        {
            // In production, resolve subdomain to TenantId from database
            _logger.LogInformation("Subdomain detected: {Subdomain}", subdomain);
        }

        return Guid.Empty;
    }
}

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = new
        {
            success = false,
            message = "An internal server error occurred",
            error = exception.Message
        };

        return context.Response.WriteAsJsonAsync(response);
    }
}

public class RateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly Dictionary<string, (int Count, DateTime ResetTime)> _clientRequests = new();
    private readonly int _maxRequests = 100;
    private readonly TimeSpan _timeWindow = TimeSpan.FromMinutes(1);

    public RateLimitMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientId = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        if (_clientRequests.TryGetValue(clientId, out var clientRequest))
        {
            if (DateTime.UtcNow < clientRequest.ResetTime)
            {
                if (clientRequest.Count >= _maxRequests)
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.Response.WriteAsJsonAsync(new { message = "Rate limit exceeded" });
                    return;
                }

                _clientRequests[clientId] = (clientRequest.Count + 1, clientRequest.ResetTime);
            }
            else
            {
                _clientRequests[clientId] = (1, DateTime.UtcNow.Add(_timeWindow));
            }
        }
        else
        {
            _clientRequests[clientId] = (1, DateTime.UtcNow.Add(_timeWindow));
        }

        await _next(context);
    }
}
