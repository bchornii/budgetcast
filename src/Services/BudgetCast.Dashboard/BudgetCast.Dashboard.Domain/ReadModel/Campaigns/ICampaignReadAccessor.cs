using System.Collections.Generic;
using System.Threading.Tasks;

namespace BudgetCast.Dashboard.Domain.ReadModel.Campaigns
{
    public interface ICampaignReadAccessor
    {
        Task<string> GetIdByName(string name);
        Task<string[]> GetCampaigns(string term, int amount);
        Task<KeyValuePair<string, string>[]> GetCampaigns(string userId);
    }
}
