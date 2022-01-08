namespace BudgetCast.Expenses.Queries.Campaigns.GetCampaignTotals
{
    public class TotalsPerCampaignVm
    {
        public decimal TotalAmount { get; set; }

        public KeyValuePair<string, decimal>[] TagTotalPair { get; set; }

        public TotalsPerCampaignVm()
        {
            TagTotalPair = default!;
        }
    }
}
