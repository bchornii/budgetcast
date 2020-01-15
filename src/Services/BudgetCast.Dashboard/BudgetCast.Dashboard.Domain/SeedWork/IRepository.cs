using System.Threading.Tasks;

namespace BudgetCast.Dashboard.Domain.SeedWork
{
    public interface IRepository<T> where T : AggregateRoot
    {
        Task<T> Add(T campaign);
        Task Update(T campaign);
        Task<T> GetAsync(string id);
    }
}