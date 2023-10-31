using BudgetCast.Common.Web.Extensions;
using BudgetCast.Common.Web.Logs;
using BudgetCast.Gateways.Bff.Extensions;
using BudgetCast.Gateways.Bff.Models;
using BudgetCast.Gateways.Bff.Services.Identity;
using BudgetCast.Gateways.Bff.Services.Session;
using BudgetCast.Gateways.Bff.Services.TokenManagement;
using BudgetCast.Gateways.Bff.Services.TokenStore;
using BudgetCast.Gateways.Bff.TransformProviders;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpLogging;
using Yarp.ReverseProxy.Configuration;

namespace BudgetCast.Gateways.Bff;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration) 
        => Configuration = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        var opts = new BffOptions();
        services.AddSingleton(opts);  
        
        var builder = services.AddReverseProxy()
            .AddTransforms<AccessTokenTransformProvider>()
            .AddTransforms<XTokenTransformProvider>();
        BuildRoutes(builder);

        services
            .AddEndpoints()
            .AddApplicationInsightsTelemetry(options =>
            {
                options.ConnectionString = Configuration
                    .GetValue<string>("BudgetCast:ApplicationInsights:ConnectionString");
            })
            .AddHttpLogging(options =>
            {
                options.LoggingFields = HttpLoggingFields.All;
                options.RequestHeaders.Add("X-Correlation-ID");
                options.RequestHeaders.Add("Authorization");
                options.ResponseHeaders.Add("WWW-Authenticate");
                options.RequestBodyLogLimit = 4096;
                options.ResponseBodyLogLimit = 4096;
            })
            .AddAuthorization()
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
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401; // Set the status code to 401
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = 403; // Set the status code to 403
                    return Task.CompletedTask;
                };
            });

        services
            .AddDistributedMemoryCache()
            .AddScoped<ISessionTrackerService, SessionTrackerService>();

        services
            .AddHttpClient<IIdentityEndpointService, IdentityEndpointService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:44305");
            });

        services
            .AddScoped<IUserAccessTokenStore, AuthenticationSessionUserAccessTokenStore>()
            .AddScoped<IUserAccessTokenManagementService, UserAccessTokenManagementService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app
            .UseApiExceptionHandling(isDevelopment: true)
            .UseHttpsRedirection()
            .UseRouting()
            .UseHttpLogging()
            .UseSharedSerilogRequestLogging()
            .UseAuthentication()
            .UseAuthorization()
            .UseEndpoints(endpoints => endpoints
                .MapEndpoints()
                .MapReverseProxy());
    }

    private void BuildRoutes(IReverseProxyBuilder builder)
    {
        builder.LoadFromMemory(
            routes: new[]
            {
                new RouteConfig
                {
                    RouteId = "expenses",
                    ClusterId = "expenses-api",
                    
                    Match = new RouteMatch()
                    {
                        Path = "/api/expenses/{**catch-all}"
                    }
                }.WithAccessToken(TokenType.User),
                new RouteConfig
                {
                    RouteId = "campaigns",
                    ClusterId = "expenses-api",
                    
                    Match = new RouteMatch
                    {
                        Path = "/api/campaigns/{**catch-all}"
                    }
                }.WithAccessToken(TokenType.User),
                new RouteConfig
                {
                    RouteId = "ext-logins",
                    ClusterId = "identity-api",
                    
                    Match = new RouteMatch
                    {
                        Path = "/api/signin/{**catch-all}"
                    }
                },
                new RouteConfig
                {
                    RouteId = "google-callback",
                    ClusterId = "identity-api",
                    
                    Match = new RouteMatch
                    {
                        Path = "/g-callback/{**catch-all}"
                    }
                },
                new RouteConfig
                {
                    RouteId = "facebook-logins",
                    ClusterId = "identity-api",
                    
                    Match = new RouteMatch
                    {
                        Path = "/fb-callback/{**catch-all}"
                    }
                }
            },
            clusters: new []
            {
                new ClusterConfig
                {
                    ClusterId = "expenses-api",
                    Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
                    {
                        {"destination1", new DestinationConfig() { Address = "http://localhost:5234"}}
                    }
                },
                new ClusterConfig
                {
                    ClusterId = "identity-api",
                    Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
                    {
                        {"destination1", new DestinationConfig() { Address = "https://localhost:44305"}}
                    }
                }
            });
    }
}