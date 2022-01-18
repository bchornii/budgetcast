namespace BudgetCast.Expenses.Queries.Expenses.GetExpenseById
{
    public class ExpenseDetailsVm
    {
        public long Id { get; set; }

        public string AddedBy { get; set; }

        public DateTime AddedAt { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Description { get; set; }

        public long CampaignId { get; set; }

        public string[] Tags { get; set; }

        public IReadOnlyList<ExpenseItemDetailsVm> ExpenseItems { get; set; }

        public ExpenseDetailsVm()
        {
            AddedBy = default!;
            Description = default!;
            Tags = default!;
            ExpenseItems = default!;
        }
    }
}
