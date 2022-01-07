namespace BudgetCast.Expenses.Queries.Campaigns
{
    public class CampaignViewModel
    {
        public ulong Id { get; set; }

        public string Name { get; set; }

        public CampaignViewModel()
        {
            Name = default!;
        }
    }
}
