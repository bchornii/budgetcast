namespace BudgetCast.Common.Domain;

public interface IRepository<TEntity, TKey> where TEntity : AggregateRoot
{
    Task<TEntity> AddAsync(TEntity campaign, CancellationToken cancellationToken);
    Task UpdateAsync(TEntity campaign, CancellationToken cancellationToken);
    Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken);
}