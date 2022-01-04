using BudgetCast.Common.Domain;

namespace BudgetCast.Expenses.Domain.Expenses
{
    public class ExpenseItem : Entity
    {
        private string _title;
        private string _note;
        private decimal _price;
        private int _quantity;

        protected ExpenseItem() 
        {
            _title = default!;
            _note = default!;
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

            _price = price;
            _quantity = quantity;
            _title = title;
        }

        public ExpenseItem(string title, decimal price,
            int quantity, string note) : this(title, price, quantity)
        {
            _note = note;
        }

        public string GetTitle() => _title;

        public string GetNote() => _note;

        public decimal GetPrice() => _price;

        public int GetQuantity() => _quantity;

        public decimal GetTotalPrice() => GetPrice() * _quantity;
    }
}
