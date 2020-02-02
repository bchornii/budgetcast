using System.Collections.Generic;
using System.Threading.Tasks;
using BudgetCast.Dashboard.Domain.ReadModel.General;

namespace BudgetCast.Dashboard.Domain.ReadModel.Receipts
{
    public interface IReceiptReadAccessor
    {
        Task<PageResult<BasicReceipt>> GetBasicReceipts(
            string campaignId, int page, int pageSize, string userId);

        Task<TotalsPerCampaign> GetTotals(string campaignId, string userId);

        Task<Receipt> GetReceiptDetails(string id);
    }
}
