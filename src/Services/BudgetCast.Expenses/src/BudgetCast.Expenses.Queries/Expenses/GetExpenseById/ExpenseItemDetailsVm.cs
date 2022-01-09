namespace BudgetCast.Expenses.Queries.Expenses.GetExpenseById
{
    public class ExpenseItemDetailsVm
    {
        public string Title { get; set; }

        public string Note { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public ExpenseItemDetailsVm()
        {
            Title = default!;
            Note = default!;
        }
    }
}
