using System.Threading.Tasks;
using BudgetCast.Dashboard.Domain.ReadModel.General;

namespace BudgetCast.Dashboard.Domain.ReadModel.Receipts
{
    public interface IReceiptReadAccessor
    {
        Task<PageResult<BasicReceipt>> GetBasicReceipts(
            string campaignId, int page, int pageSize);
    }
}
