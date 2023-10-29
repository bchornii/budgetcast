using BudgetCast.Common.Web.Logs;
using BudgetCast.Gateways.Bff;
using Serilog;
using Serilog.Events;

public static class Program
{
    public static int Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateBootstrapLogger();;

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

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSharedSerilogConfiguration()          
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}