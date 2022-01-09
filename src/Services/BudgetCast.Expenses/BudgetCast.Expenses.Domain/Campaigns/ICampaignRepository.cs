using BudgetCast.Common.Domain;

namespace BudgetCast.Expenses.Domain.Campaigns
{
    public interface ICampaignRepository : IRepository<Campaign, long>
    {
        Task<Campaign?> GetByNameAsync(string name, CancellationToken cancellationToken);

        Task<bool> ExistsAsync(string name, CancellationToken cancellationToken);
    }
}
