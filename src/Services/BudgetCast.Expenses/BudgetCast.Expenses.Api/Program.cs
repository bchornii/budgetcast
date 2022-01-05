using BudgetCast.Expenses.Api;
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

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog((ctx, services, configuration) =>
                configuration.ReadFrom.Configuration(ctx.Configuration))
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

    private static Serilog.ILogger CreateSerilogLogger()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateBootstrapLogger();
    }
}
