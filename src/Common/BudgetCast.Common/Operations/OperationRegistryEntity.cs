namespace BudgetCast.Common.Operations;

public class OperationRegistryEntity
{
    public int Id { get; set; }

    public ICollection<OperationPart> Operations { get; init; } = default!;

    public string IdempodentOperationName { get; init; } = default!;

    public DateTime StartedOn { get; init; }

    public string CorrelationId { get; init; } = default!;

    public string OperationResult { get; set; } = default!;
}