using BudgetCast.Common.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BudgetCast.Common.Data;

public class OperationsDal : IOperationsDal
{
    private readonly DbContext _dbContext;
    private readonly ILogger<OperationsDal> _logger;

    public OperationsDal(DbContext context, ILogger<OperationsDal> logger)
    {
        _dbContext = context;
        _logger = logger;
    }

    public async Task<string> CleanAsync(CancellationToken cancellationToken)
    {
        var tableName = _dbContext.Model
            .FindEntityType(typeof(OperationRegistryEntity))!.GetTableName();

        _logger.LogInformation("Start 'TRUNCATE TABLE {TableName}' operation", tableName);
        await _dbContext.Database.OpenConnectionAsync(cancellationToken);
        await _dbContext.Database.ExecuteSqlRawAsync($"TRUNCATE TABLE {tableName}", cancellationToken);
        _logger.LogInformation("'TRUNCATE TABLE {TableName}' operation completed", tableName);

        return tableName!;
    }
}