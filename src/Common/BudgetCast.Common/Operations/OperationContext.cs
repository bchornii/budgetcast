using System.Globalization;

namespace BudgetCast.Common.Operations;

public sealed class OperationContext
{
    public const string HeaderName = "OperationContext";

    /// <summary>
    /// A unique Id, which is stays the same for all commands in a complex operation
    /// </summary>
    public string CorrelationId { get; }

    /// <summary>
    /// A point in time when the operation was started.
    /// Often that would be Blazor.UI code as the initiator of complex operations
    /// </summary>
    public DateTime StartedOn { get; }

    /// <summary>
    /// Contains a collection of all operation parts tracked from the start of a complex operation
    /// </summary>
    public IReadOnlyCollection<OperationPart> OperationParts => _operationParts;

    /// <summary>
    /// The last performed operation
    /// </summary>
    public OperationPart IdempodentOperation { get; private set; } = default!;

    private OperationContext(string correlationId, DateTime startedOn)
    {
        CorrelationId = correlationId;
        StartedOn = startedOn;
        _operationParts = new List<OperationPart>();
    }

    public static OperationContext New() 
        => new(Guid.NewGuid().ToString("N"), DateTime.UtcNow);

    public static OperationContext New(string correlationId) 
        => new(correlationId, DateTime.UtcNow);

    public static OperationContext Unpack(string packedString)
    {
        var parts = packedString.Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);
        var correlationId = parts[0];
        var startedOnTicks = long.Parse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture);

        var context = new OperationContext(
            correlationId, 
            new DateTime(startedOnTicks, DateTimeKind.Utc));

        if (parts.Length > 2)
        {
            var operationPartNames = parts[2].Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);

            foreach (var operationPartName in operationPartNames)
            {
                context.Add(new OperationPart(operationPartName));
            }
        }

        return context;
    }

    public void Add(OperationPart operationPart)
    {
        _operationParts.Add(operationPart);
        IdempodentOperation = operationPart;
    }

    public string Pack()
    {
        if (_operationParts.Any())
        {
            var operationParts = string.Join("|", _operationParts.Select(o => o.Name));
            return $"{CorrelationId:N}:{StartedOn.Ticks}:{operationParts}";
        }

        return $"{CorrelationId:N}:{StartedOn.Ticks}";
    }

    public override string ToString()
        => $"The operation with CorrelationId: {CorrelationId} was started on {StartedOn}. {Environment.NewLine}" +
           $"Operation parts: {string.Join(",", OperationParts.Select(p => p.Name))}";

    private readonly List<OperationPart> _operationParts;
}