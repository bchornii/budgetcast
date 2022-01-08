namespace BudgetCast.Expenses.Queries.Expenses.GetCampaingExpenses
{
    public class ExpenseVm
    {
        public long Id { get; set; }

        public string CreatedBy { get; set; }

        public DateTime AddedAt { get; set; }

        public string Description { get; set; }

        public int? TotalItems { get; set; }

        public decimal? TotalAmount { get; set; }

        public string[] Tags { get; set; }

        public ExpenseVm()
        {
            Description = default!;
            Tags = default!;
            CreatedBy = default!;
        }
    }
}
