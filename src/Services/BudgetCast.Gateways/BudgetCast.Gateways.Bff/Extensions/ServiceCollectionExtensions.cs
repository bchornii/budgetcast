using BudgetCast.Gateways.Bff.Endpoints;
using BudgetCast.Gateways.Bff.Services.Session;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BudgetCast.Gateways.Bff.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        var endpoints = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(t => t.GetInterfaces().Contains(typeof(IEndpoint)))
            .Where(t => !t.IsInterface);

        foreach (var endpoint in endpoints)
        {
            services.AddScoped(typeof(IEndpoint), endpoint);
        }

        return services;
    }

    /// <summary>
    /// Add cookie based authentication with custom defined cookie events.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="opts"></param>
    /// <returns></returns>
    public static IServiceCollection AddCookieAuthentication(this IServiceCollection services, BffOptions opts)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = opts.BffAuthenticationCookieName;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(14);

                // Revoke the session on signout time in order to eliminate issues with non-expired cached cookies.
                options.Events.OnSigningOut = async context =>
                {
                    var logger = context.HttpContext
                        .RequestServices.GetService<ILogger<CookieAuthenticationHandler>>();

                    var uuid = context.HttpContext.User.GetUuid();
                    if (uuid.IsPresent())
                    {
                        var sessionTracker = context.HttpContext
                            .RequestServices.GetService<ISessionTrackerService>();
                        if (sessionTracker is not null)
                        {
                            logger.LogDebug("Revoked principal with {uuid} id", uuid);
                            await sessionTracker.RevokeAsync(uuid!);
                        }
                    }
                };

                // Validate session for expiration to eliminate usage of cached non-expired cookies.
                options.Events.OnValidatePrincipal = async context =>
                {
                    var logger = context.HttpContext
                        .RequestServices.GetService<ILogger<CookieAuthenticationHandler>>();

                    var uuid = context.Principal.GetUuid();
                    logger?.LogDebug("Principal validation for {uuid}", uuid);
                    if (uuid.IsPresent())
                    {
                        var sessionTracker = context.HttpContext
                            .RequestServices.GetService<ISessionTrackerService>();
                        if (sessionTracker is not null)
                        {
                            if (await sessionTracker.IsRevokedAsync(uuid!))
                            {
                                context.RejectPrincipal();
                                logger?.LogWarning("Principal with {uuid} id rejected", uuid);
                            }
                            else
                            {
                                logger?.LogDebug("Principal with {uuid} id is valid", uuid);
                            }
                        }
                    }
                };
                
                // OnRedirectToLogin to login return 401 instead of actual redirect
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
                
                // OnRedirectToAccessDenied to login return 403 instead of actual redirect
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = 403;
                    return Task.CompletedTask;
                };
            });
        
        return services;
    }
}