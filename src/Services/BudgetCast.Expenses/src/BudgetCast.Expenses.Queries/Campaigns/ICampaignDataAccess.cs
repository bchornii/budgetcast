namespace BudgetCast.Expenses.Queries.Campaigns
{
    public interface ICampaignDataAccess
    {
        Task<CampaignVm> GetAsync(string campaignName, CancellationToken cancellationToken);
    }
}
