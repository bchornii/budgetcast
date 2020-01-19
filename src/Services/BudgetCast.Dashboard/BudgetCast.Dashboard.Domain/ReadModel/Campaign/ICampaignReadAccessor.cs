﻿using System.Threading.Tasks;

namespace BudgetCast.Dashboard.Domain.ReadModel.Campaign
{
    public interface ICampaignReadAccessor
    {
        Task<string> GetIdByName(string name);
        Task<string[]> GetCampaigns(string term, int amount);
    }
}