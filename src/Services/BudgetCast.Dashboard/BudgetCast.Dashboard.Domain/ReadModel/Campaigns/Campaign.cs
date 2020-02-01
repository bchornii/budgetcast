using System;
using BudgetCast.Dashboard.Domain.SeedWork;

namespace BudgetCast.Dashboard.Domain.ReadModel.Campaigns
{
    public class Campaign : HistoricalReadModel
    {
        public string Name { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime CompletesAt { get; set; }
    }
}