using System.Threading.Tasks;

namespace BudgetCast.Dashboard.Domain.SeedWork
{
    public interface IUnitOfWork
    {
        Task<bool> Commit();
    }
}