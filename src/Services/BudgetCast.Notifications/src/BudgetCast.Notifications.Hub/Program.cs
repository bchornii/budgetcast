using BudgetCast.Common.Web.Extensions;
using BudgetCast.Notifications.AppHub.Hubs;
using BudgetCast.Notifications.AppHub.Infrastructure.Extensions;
using BudgetCast.Notifications.AppHub.Middlewares;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Events;

var logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
Log.Logger = logger;

Log.Information("Populating services in DI container");
builder
    .Services
    .AddWebAppServices(configuration);

Log.Information("Configuring web host");
builder
    .Host
    .AddWebAppHostConfiguration();

try
{
    Log.Information("Building web host");
    var app = builder.Build();
    Log.Information("Web host was successfully built");
    
    Log.Information("Configuring web app pipeline");
    var env = app.Environment;
    
    app.UseApiExceptionHandling(isDevelopment: false);
            
    app.UseHttpsRedirection();

    app.UseCors();

    app.UseAuthentication();
    app.UseCurrentTenant();
    app.UseAuthorization();

    if (env.IsDevelopment())
    {
        app.UseTestEndpoints();
    }            

    app.MapHub<NotificationHub>("/hubs/notifications", options =>
    {
        //options.CloseOnAuthenticationExpiration = true;
    });

    app.MapHealthChecks("/hc", new HealthCheckOptions()
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
        AllowCachingResponses = false
    });
    app.MapHealthChecks("/liveness", new HealthCheckOptions
    {
        Predicate = r => r.Name.Contains("self"),
        AllowCachingResponses = false
    });
    Log.Information("Web app pipeline configured");
    
    Log.Information("Starting web host");
    await app.RunAsync();
    Log.Information("Stopping web host");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}