namespace BudgetCast.Common.Operations;

public interface IOperationsDal
{
    Task<string> CleanAsync(CancellationToken cancellationToken);
}