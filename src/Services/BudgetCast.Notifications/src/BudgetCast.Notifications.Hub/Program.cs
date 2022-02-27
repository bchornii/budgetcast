using BudgetCast.Common.Messaging.Azure.ServiceBus.Extensions;
using BudgetCast.Notifications.AppHub;
using BudgetCast.Notifications.AppHub.EventHandlers;
using Serilog;
using Serilog.Events;

public class Program
{
    public static int Main(string[] args)
    {
        Log.Logger = CreateSerilogLogger();

        try
        {
            Log.Information("Configuring web host");
            var host = CreateHostBuilder(args).Build();

            Log.Information("Starting web host");
            host.Run();

            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog((ctx, services, configuration) =>
                configuration.ReadFrom.Configuration(ctx.Configuration))
            .UseAzServiceBus(
                registerHandlers: services =>
                {
                    services.AddScoped<TestIntegrationEventHandler>();
                },
                subscribeToEvents: processor =>
                {
                    processor.SubscribeTo<TestIntegrationEvent, TestIntegrationEventHandler>();
                })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

    private static Serilog.ILogger CreateSerilogLogger() =>
        new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateBootstrapLogger();
}
