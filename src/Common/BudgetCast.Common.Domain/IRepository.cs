namespace BudgetCast.Common.Domain
{
    public interface IRepository<T> where T : AggregateRoot
    {
        Task<T> Add(T campaign);
        Task Update(T campaign);
        Task<T> GetAsync(string id);
    }
}
