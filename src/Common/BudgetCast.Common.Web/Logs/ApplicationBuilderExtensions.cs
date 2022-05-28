using Microsoft.AspNetCore.Builder;
using Serilog;

namespace BudgetCast.Common.Web.Logs;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseSharedSerilogRequestLogging(this IApplicationBuilder builder)
    {
        return builder
            .UseSerilogRequestLogging(options =>
            {
                options.IncludeQueryInRequestPath = true;
                options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms.";
            });
    }
}