namespace BudgetCast.Expenses.Queries.Campaigns
{
    public interface ICampaignDataAccess
    {
        Task<IReadOnlyList<CampaignVm>> GetAsync(int amount, string campaignName, CancellationToken cancellationToken);

        Task<CampaignVm> GetAsync(string campaignName, CancellationToken cancellationToken);
    }
}
