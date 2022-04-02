using BudgetCast.Common.Operations;
using Microsoft.EntityFrameworkCore;

namespace BudgetCast.Common.Data;

public class MsSqlOperationsRegistry : IOperationsRegistry
{
    private readonly OperationContext _operationContext;

    private readonly DbContext _dbContext;

    public MsSqlOperationsRegistry(OperationContext operationContext, DbContext dbContext)
    {
        _operationContext = operationContext;
        _dbContext = dbContext;
    }

    public async Task<(bool IsOperationExists, string OperationResult)> TryAddCurrentOperationAsync()
    {
        var operation = await GetOperationAsync();

        if (operation != null)
        {
            return (true,
                string.IsNullOrWhiteSpace(operation.OperationResult) ?
                    string.Empty : operation.OperationResult);
        }

        _dbContext
            .Set<OperationRegistryEntity>()
            .Add(OperationRegistryMapper.MapOperationRegistry(_operationContext));

        return (false, string.Empty);
    }

    public Task SetCurrentOperationCompletedAsync() => Task.CompletedTask;

    public async Task SetCurrentOperationCompletedAsync(string result)
    {
        if (string.IsNullOrWhiteSpace(result))
        {
            return;
        }

        var operation = await GetOperationAsync();

        if (operation != null)
        {
            operation.OperationResult = result;
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task<OperationRegistryEntity?> GetOperationAsync() =>
        await _dbContext
            .Set<OperationRegistryEntity>()
            .Where(s => s.CorrelationId == _operationContext.CorrelationId.ToString()
                        && s.IdempodentOperationName == _operationContext.IdempodentOperation.Name)
            .FirstOrDefaultAsync(CancellationToken.None);
}