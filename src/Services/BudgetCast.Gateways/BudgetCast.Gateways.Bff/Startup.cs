using BudgetCast.Common.Web.Extensions;
using BudgetCast.Common.Web.Logs;
using BudgetCast.Gateways.Bff.Extensions;
using BudgetCast.Gateways.Bff.Middleware;
using BudgetCast.Gateways.Bff.Services.Identity;
using BudgetCast.Gateways.Bff.Services.Session;
using BudgetCast.Gateways.Bff.Services.TokenManagement;
using BudgetCast.Gateways.Bff.Services.TokenStore;
using BudgetCast.Gateways.Bff.TransformProviders;
using Microsoft.AspNetCore.HttpLogging;

namespace BudgetCast.Gateways.Bff;

public class Startup
{
    private IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration) 
        => Configuration = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        var opts = new BffOptions();
        services.AddSingleton(opts);

        var appSettings = new AppSettings();
        Configuration.Bind(appSettings);
        services.AddSingleton(appSettings);
        
        var builder = services.AddReverseProxy()
            .AddTransforms<AccessTokenTransformProvider>()
            .AddTransforms<XTokenTransformProvider>();
        builder.BuildApplicationRoutes(appSettings);

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
            .AddCookieAuthentication(opts);

        services
            .AddDistributedMemoryCache()
            .AddScoped<ISessionTrackerService, SessionTrackerService>();

        services
            .AddHttpClient<IIdentityEndpointService, IdentityEndpointService>(client =>
            {
                client.BaseAddress = new Uri(appSettings.IdentityApiUrl);
            });

        services
            .AddScoped<IUserAccessTokenStore, AuthenticationSessionUserAccessTokenStore>()
            .AddScoped<IUserAccessTokenManagementService, UserAccessTokenManagementService>()
            .AddSingleton<IUserAccessTokenRequestSynchronization, AccessTokenRequestSynchronization>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) =>
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
                .MapReverseProxy(proxy => proxy
                    .UseMiddleware<AntiforgeryMiddleware>()));
}