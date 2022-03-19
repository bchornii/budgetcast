using Microsoft.Extensions.Logging;

namespace BudgetCast.Common.Extensions;

public static class LoggerExtensions
{
    public static ILogger LogInformationIfEnabled(this ILogger logger, string? message, object arg1)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(message, arg1);
        }
        return logger;
    }
    
    public static ILogger LogInformationIfEnabled(this ILogger logger, string? message, object arg1, object arg2)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(message, arg1, arg2);
        }
        return logger;
    }
    
    public static ILogger LogInformationIfEnabled(this ILogger logger, string? message, object arg1, object arg2, object arg3)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(message, arg1, arg2, arg3);
        }
        return logger;
    }
    
    public static ILogger LogDebugIfEnabled(this ILogger logger, string? message, params object?[] args)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug(message, args);
        }
        return logger;
    }
}