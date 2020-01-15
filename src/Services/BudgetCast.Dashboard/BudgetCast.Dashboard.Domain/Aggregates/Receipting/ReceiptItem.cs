using BudgetCast.Dashboard.Domain.Exceptions;
using BudgetCast.Dashboard.Domain.SeedWork;

namespace BudgetCast.Dashboard.Domain.Aggregates.Receipting
{
    public class ReceiptItem : Entity
    {
        private string _title;
        private string _description;
        private decimal _price;
        private int _quantity;

        protected ReceiptItem() { }

        public ReceiptItem(string title, decimal price, int quantity) : this()
        {
            if (price < 0)
            {
                throw new ReceiptDomainException("Receipt item price should be greater that 0.");
            }

            if (quantity < 1 && quantity > 1000)
            {
                throw new ReceiptDomainException("Receipt item quantity should be between 1 and 1000");
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ReceiptDomainException("Receipt item should have title set.");
            }

            _price = price;
            _quantity = quantity;
            _title = title;
        }

        public ReceiptItem(string title, decimal price,
            int quantity, string description) : this(title, price, quantity)
        {
            _description = description;
        }

        public string GetTitle() => _title;

        public string GetDescription() => _description;

        public decimal GetSingleItemPrice() => _price;

        public decimal GetPrice() => GetSingleItemPrice() * _quantity;
    }
}