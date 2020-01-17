using System.Threading.Tasks;
using BudgetCast.Dashboard.Data;
using BudgetCast.Dashboard.Domain.AnemicModel;

namespace BudgetCast.Dashboard.WriteAccessors
{
    public class DefaultTagWriteAccessor : IDefaultTagWriteAccessor
    {
        private readonly BudgetCastContext _context;

        public DefaultTagWriteAccessor(BudgetCastContext context)
        {
            _context = context;
        }

        public Task AddTags(DefaultTag[] tags)
        {
            return _context.DefaultTags.InsertManyAsync(tags);
        }
    }
}
