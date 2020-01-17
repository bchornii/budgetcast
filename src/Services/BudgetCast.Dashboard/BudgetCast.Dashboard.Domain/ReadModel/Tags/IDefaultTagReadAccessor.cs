using System.Collections.Generic;
using System.Threading.Tasks;

namespace BudgetCast.Dashboard.Domain.ReadModel.Tags
{
    public interface IDefaultTagReadAccessor
    {
        Task<IReadOnlyList<string>> GetExistingTags(string[] tags);
        Task<IReadOnlyList<string>> GetTags(string userId, string term, int amount);
    }
}
