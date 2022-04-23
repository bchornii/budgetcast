namespace BudgetCast.Common.Operations;

public static class OperationRegistryMapper
{
    public static OperationRegistryEntry MapOperationRegistry(OperationContext context)
    {
        return new OperationRegistryEntry
        {
            Operations = context.OperationParts.ToList(),
            StartedOn = context.StartedOn,
            CorrelationId = context.CorrelationId,
            IdempodentOperationName = context.IdempodentOperation.Name,
            OperationResult = string.Empty,
        };
    }
}