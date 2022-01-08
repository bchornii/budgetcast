namespace BudgetCast.Expenses.Queries.Campaigns
{
    public class CampaignVm
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public CampaignVm()
        {
            Name = default!;
        }
    }
}
