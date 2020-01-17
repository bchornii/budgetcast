using System.Threading.Tasks;

namespace BudgetCast.Dashboard.Domain.AnemicModel
{
    public interface IDefaultTagWriteAccessor
    {
        Task AddTags(DefaultTag[] tags);
    }
}
