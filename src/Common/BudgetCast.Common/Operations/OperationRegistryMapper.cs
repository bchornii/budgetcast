namespace BudgetCast.Common.Operations;

public static class OperationRegistryMapper
{
    public static OperationRegistryEntity MapOperationRegistry(OperationContext context)
    {
        return new OperationRegistryEntity
        {
            Operations = context.OperationParts.ToList(),
            StartedOn = context.StartedOn,
            CorrelationId = context.CorrelationId,
            IdempodentOperationName = context.IdempodentOperation.Name,
            OperationResult = string.Empty,
        };
    }
}