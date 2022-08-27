using BudgetCast.Common.Domain;
using BudgetCast.Common.Domain.Results;

namespace BudgetCast.Expenses.Domain.Expenses
{
    public class ExpenseItem : Entity
    {
        public string Title { get; private set; }

        public string? Note { get; private set; }

        public decimal Price { get; private set; }

        public int Quantity { get; private set; }
        
        public string BarCode { get; private set; }

        protected ExpenseItem() 
        {
            Title = default!;
            Note = default!;
            BarCode = default!;
        }

        public ExpenseItem(string title, decimal price, int quantity = 1) : this()
        {
            if (price < 0)
            {
                throw new Exception("Expense item price should be greater that 0.");
            }

            if (quantity < 1 || quantity > 1000)
            {
                throw new Exception("Expense item quantity should be between 1 and 1000");
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new Exception("Expense item should have title set.");
            }

            Price = price;
            Quantity = quantity;
            Title = title;
        }

        public static Result<ExpenseItem> Create(string title, decimal price, int quantity = 1)
        {
            if (price < 0)
            {
                return InvariantViolations.Expenses.ExpenseItems.ExpenseItemPriceIsZero();
            }

            if (quantity < 1 || quantity > 1000)
            {
                return InvariantViolations.Expenses.ExpenseItems.ItemsQuantityIsMoreThan1000();
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                return InvariantViolations.Expenses.ExpenseItems.ItemShouldHaveTitle();
            }

            return new ExpenseItem(title, price, quantity);
        }

        public ExpenseItem(string title, decimal price,
            int quantity, string note) : this(title, price, quantity)
        {
            Note = note;
        }

        public static Result<ExpenseItem> Create(string title, decimal price,
            int quantity, string note)
        {
            return new ExpenseItem(title, price, quantity, note);
        }

        public Result UpdateNote(string note)
        {
            if(string.IsNullOrWhiteSpace(note))
            {
                return InvariantViolations.Expenses.ExpenseItems.NoteShouldHaveText();
            }
            Note = note;
            
            return Success.Empty;
        }

        public string GetTitle() => Title;

        public string? GetNote() => Note;

        public decimal GetPrice() => Price;

        public int GetQuantity() => Quantity;

        public decimal GetTotalPrice() => GetPrice() * Quantity;
    }
}
