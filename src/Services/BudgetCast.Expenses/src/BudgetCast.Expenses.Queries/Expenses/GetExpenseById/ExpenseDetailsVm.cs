namespace BudgetCast.Expenses.Queries.Expenses.GetExpenseById
{
    public class ExpenseDetailsVm
    {
        public DateTime AddedAt { get; set; }

        public string Description { get; set; }

        public long CampaignId { get; set; }

        public string[] Tags { get; set; }

        public IReadOnlyList<ExpenseItemDetailsVm> ExpenseItems { get; set; }

        public ExpenseDetailsVm()
        {
            Description = default!;
            Tags = default!;
            ExpenseItems = default!;
        }
    }
}
