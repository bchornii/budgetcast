namespace BudgetCast.Common.Domain
{
    public interface IRepository<TEntity, TKey> where TEntity : AggregateRoot
    {
        Task<TEntity> Add(TEntity campaign, CancellationToken cancellationToken);
        Task Update(TEntity campaign, CancellationToken cancellationToken);
        Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken);
    }
}
