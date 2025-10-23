using BudgetCast.Gateways.Bff.Extensions;
using BudgetCast.Gateways.Bff.Models;

namespace BudgetCast.Gateways.Bff.Middleware;

/// <summary>
/// Middleware for YARP to check the antiforgery header
/// <remarks>CSRF header is important for protection against sub-domain hijacking attack vector. Sub-domain hijacking
/// (or sub-domain takeover) occurs when an attacker is able to claim an abandoned web host that still has valid DNS
/// records configured. Combination of SameSite cookie and CSRF header combo is a sophisticated protection.</remarks>
/// </summary>
public class AntiforgeryMiddleware(RequestDelegate next, BffOptions options, ILogger<AntiforgeryMiddleware> logger)
{
    private static readonly string True = true.ToString();

    /// <summary>
    /// Get invoked for YARP requests
    /// </summary>
    /// <param name="context"></param>
    public async Task Invoke(HttpContext context)
    {
        var route = context.GetRouteModel();

        if (route.Config.Metadata != null)
        {
            if (route.Config.Metadata.TryGetValue(AppConstants.Yarp.AntiforgeryCheckMetadata, out var value))
            {
                if (string.Equals(value, True, StringComparison.OrdinalIgnoreCase))
                {
                    if (!context.CheckAntiForgeryHeader(options))
                    {
                        context.Response.StatusCode = 401;
                        logger.LogWarning("CSRF token is missing from the request to {routeId}", route.Config.RouteId);
                        
                        return;
                    }
                }
            }
        }
        
        await next(context);
    }
}