using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.Destructurers;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Sinks.SystemConsole.Themes;

namespace BudgetCast.Common.Web.Logs;

public static class HostBuilderExtensions
{
    public static IHostBuilder UseSharedSerilogConfiguration(this IHostBuilder builder)
    {
        return builder.UseSerilog((context, services, configuration) =>
        {
            var serviceName = context.Configuration.GetValue<string>("ServiceName");

            configuration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithSpan()
                .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                    .WithDefaultDestructurers()
                    .WithDestructurers(new IExceptionDestructurer[]
                    {
                        new DbUpdateExceptionDestructurer()
                    }))
                .Enrich.WithMachineName()
                .Enrich.WithCorrelationIdHeader("X-Correlation-ID")
                .Enrich.With(new OperationIdEnricher())
                .Enrich.WithProperty("ServiceName", serviceName);

            if (context.HostingEnvironment.IsDevelopment())
            {
                configuration
                    .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information, theme: AnsiConsoleTheme.Code)
                    .WriteTo.Seq("http://localhost:5341", restrictedToMinimumLevel: LogEventLevel.Information);
            }
            else
            {
                configuration
                    .WriteTo.Async(x => x.Console(
                        restrictedToMinimumLevel: LogEventLevel.Information, 
                        theme: AnsiConsoleTheme.Code));
            }
            
            configuration
                .WriteTo.Async(x => x.ApplicationInsights(
                        services.GetRequiredService<TelemetryConfiguration>(),
                        TelemetryConverter.Traces,
                        restrictedToMinimumLevel: LogEventLevel.Information),
                    bufferSize: 10);
        });
    }
}