namespace BudgetCast.Common.Domain
{
    public interface IRepository<TEntity, TKey> where TEntity : AggregateRoot
    {
        Task<TEntity> Add(TEntity campaign);
        Task Update(TEntity campaign);
        Task<TEntity> GetAsync(TKey id);
    }
}
