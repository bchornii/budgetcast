using System.Text.Json;
using BudgetCast.Common.Extensions;
using Microsoft.Extensions.Caching.Distributed;

namespace BudgetCast.Common.Operations;

/// <summary>
/// Stores operation registry entries in memory for 12 hours (absolute expiration).
/// </summary>
public sealed class InMemoryOperationRegistry : IOperationsRegistry
{
    private readonly IDistributedCache _cache;
    private readonly OperationContext _operationContext;
    private readonly DistributedCacheEntryOptions _cacheEntryOptions;

    public InMemoryOperationRegistry(IDistributedCache cache, OperationContext operationContext)
    {
        _cache = cache;
        _operationContext = operationContext;
        _cacheEntryOptions = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromHours(12));
    }
    
    public async Task<(bool IsOperationExists, string OperationResult)> TryAddCurrentOperationAsync(
        CancellationToken cancellationToken)
    {
        var key = GetKey();
        var existingOperationEntityJson = await _cache.GetStringAsync(key, cancellationToken);

        if (!string.IsNullOrWhiteSpace(existingOperationEntityJson))
        {
            var existingOperationEntity = JsonSerializer
                .Deserialize(existingOperationEntityJson, typeof(OperationRegistryEntity));
            
            if (existingOperationEntity is OperationRegistryEntity entity)
            {
                var operationResult = entity.OperationResult;
                return (true, operationResult.ToEmptyIfNull());
            }
        }

        var operationEntity = OperationRegistryMapper.MapOperationRegistry(_operationContext);
        var operationEntityJson = JsonSerializer
            .Serialize(operationEntity, typeof(OperationRegistryEntity));
        await _cache.SetStringAsync(key, operationEntityJson, _cacheEntryOptions, cancellationToken);

        return (false, string.Empty);
    }

    public Task SetCurrentOperationCompletedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public async Task SetCurrentOperationCompletedAsync(string result, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(result))
        {
            return;
        }

        var key = GetKey();
        var existingOperationEntityJson = await _cache.GetStringAsync(key, cancellationToken);

        if (existingOperationEntityJson is not null)
        {
            var existingOperationEntity = JsonSerializer
                .Deserialize(existingOperationEntityJson, typeof(OperationRegistryEntity));
            
            if (existingOperationEntity is OperationRegistryEntity entity)
            {
                entity.OperationResult = result;
                
                var operationEntityJson = JsonSerializer
                    .Serialize(entity, typeof(OperationRegistryEntity));
                await _cache.SetStringAsync(key, operationEntityJson, _cacheEntryOptions, cancellationToken);
            }
        }
    }

    private string GetKey()
        => $"{_operationContext.CorrelationId}:{_operationContext.IdempodentOperation.Name}";
}