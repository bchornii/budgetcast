using System.Threading.Tasks;

namespace BudgetCast.Dashboard.Compensations
{
    public interface ICompensationAction
    {
        Task Compensate();
    }
}