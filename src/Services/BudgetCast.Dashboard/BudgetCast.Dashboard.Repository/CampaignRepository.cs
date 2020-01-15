using BudgetCast.Dashboard.Data;
using BudgetCast.Dashboard.Domain.Aggregates.Campaigns;

namespace BudgetCast.Dashboard.Repository
{
    public class CampaignRepository : Repository<Campaign>, ICampaignRepository
    {
        public CampaignRepository(BudgetCastContext context) : base(context) { }
    }
}
