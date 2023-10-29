using BudgetCast.Common.Web.Extensions;
using BudgetCast.Common.Web.Logs;
using BudgetCast.Gateways.Bff.Extensions;
using BudgetCast.Gateways.Bff.Models;
using BudgetCast.Gateways.Bff.Services;
using BudgetCast.Gateways.Bff.TransformProviders;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpLogging;
using Yarp.ReverseProxy.Configuration;

namespace BudgetCast.Gateways.Bff;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    
    public void ConfigureServices(IServiceCollection services)
    {
        var builder = services.AddReverseProxy()
            .AddTransforms<AccessTokenTransformProvider>()
            .AddTransforms<LoggingTransformProvider>();

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
                }.WithAccessToken(TokenType.User)
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
                }
            });
        
        services.AddControllers();
        
        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = "__BudgetCast-gw-bff";
                options.Cookie.SameSite = SameSiteMode.Strict;
                
                // automatically revoke refresh token at signout time
                options.Events.OnSigningOut = async e => { await Task.CompletedTask; };
            });
        
        services.AddApplicationInsightsTelemetry(options =>
        {
            options.ConnectionString = Configuration
                .GetValue<string>("BudgetCast:ApplicationInsights:ConnectionString");
        });

        services.AddHttpLogging(options =>
        {
            options.LoggingFields = HttpLoggingFields.All;
            options.RequestHeaders.Add("X-Correlation-ID");
            options.RequestHeaders.Add("Authorization");
            options.ResponseHeaders.Add("WWW-Authenticate");
            options.RequestBodyLogLimit = 4096;
            options.ResponseBodyLogLimit = 4096;
        });

        services.AddHttpClient<IIdentityEndpointService, IdentityEndpointService>(client =>
        {
            client.BaseAddress = new Uri("https://localhost:44305");
        });

        services.AddScoped<IUserAccessTokenStore, AuthenticationSessionUserAccessTokenStore>();
        services.AddScoped<IUserAccessTokenManagementService, UserAccessTokenManagementService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseApiExceptionHandling(isDevelopment: true);
        app.UseHttpsRedirection();
        
        app.UseRouting();
        app.UseHttpLogging();
        app.UseSharedSerilogRequestLogging();
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseEndpoints(endpoints =>
        {
            //endpoints.MapBffManagementEndpoints();
                
            // if you want the TODOs API local
            // endpoints.MapControllers()
            //     .RequireAuthorization()
            //     .AsBffApiEndpoint();

            // if you want the TODOs API remote
            //endpoints.MapBffReverseProxy();

            endpoints.MapControllers();
            
            endpoints.MapReverseProxy();

            // which is equivalent to
            //endpoints.MapReverseProxy()
            //    .AsBffApiEndpoint();
        });
    }
}