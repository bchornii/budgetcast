using Serilog.Core;
using Serilog.Events;

namespace BudgetCast.Common.Web.Logs;

public class OperationIdEnricher : ILogEventEnricher
{
    public const string OperationId = "operationId";
    
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (!logEvent.Properties.ContainsKey(OperationId) && 
            logEvent.Properties.TryGetValue("TraceId", out var traceId))
        {
            logEvent.AddPropertyIfAbsent(new LogEventProperty(OperationId, traceId));
        }
    }
}