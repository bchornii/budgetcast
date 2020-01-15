using System.Threading.Tasks;
using BudgetCast.Dashboard.Data;
using BudgetCast.Dashboard.Domain.Aggregates.Receipting;
using MongoDB.Driver;

namespace BudgetCast.Dashboard.Repository
{
    public class ReceiptRepository : Repository<Receipt>, IReceiptRepository
    {
        private readonly BudgetCastContext _context;

        public ReceiptRepository(BudgetCastContext context) : base(context)
        {
            _context = context;
        }
    }
}
