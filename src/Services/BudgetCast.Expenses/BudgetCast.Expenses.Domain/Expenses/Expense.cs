using BudgetCast.Common.Domain;

namespace BudgetCast.Expenses.Domain.Expenses
{
    public class Expense : AggregateRoot
    {
        private DateTime _date;
        private string _description;

        private string _campaignId;
        public string CampaignId => _campaignId;

        private readonly List<ExpenseItem> _expenseItems;
        public IReadOnlyCollection<ExpenseItem> ExpenseItems => _expenseItems;

        private readonly List<Tag> _tags;
        public IReadOnlyCollection<Tag> Tags => _tags;

        protected Expense()
        {
            _description = default!;
            _campaignId = default!;
            _expenseItems = new List<ExpenseItem>();
            _tags = new List<Tag>();
        }

        public Expense(DateTime date, 
            string campaignId,
            string description = "no description") : this()
        {
            _date = date;
            _campaignId = campaignId;
            _description = description;
        }
        public virtual void AddItem(ExpenseItem expenseItem)
        {
            if (_expenseItems.Count >= 100)
            {
                throw new Exception("Receipt can't hold more than 1000 items.'");
            }

            _expenseItems.Add(expenseItem);
        }

        public virtual void AddTags(Tag[] tags)
        {
            if ((_tags.Count + tags.Length) > 100)
            {
                throw new Exception("Receipt can't have more than 10 tags.");
            }

            var nonExistingTags = tags
                .Where(t => !_tags.Contains(t)).ToArray();
            _tags.AddRange(nonExistingTags);
        }

        public virtual void SetCampaignId(string campaignId)
        {
            _campaignId = campaignId;
        }

        public virtual decimal TotalAmount() => _expenseItems.Sum(item => item.GetTotalPrice());
    }
}
