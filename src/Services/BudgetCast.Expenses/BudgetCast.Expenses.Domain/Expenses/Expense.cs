using BudgetCast.Common.Domain;

namespace BudgetCast.Expenses.Domain.Expenses
{
    public class Expense : AggregateRoot
    {
        public DateTime AddedAt { get; private set; }

        public string Description { get; private set; }

        public string CampaignId { get; private set; }

        private readonly List<ExpenseItem> _expenseItems;
        public IReadOnlyCollection<ExpenseItem> ExpenseItems => _expenseItems;

        private readonly List<Tag> _tags;
        public IReadOnlyCollection<Tag> Tags => _tags;

        protected Expense()
        {
            Description = default!;
            CampaignId = default!;
            _expenseItems = new List<ExpenseItem>();
            _tags = new List<Tag>();
        }

        public Expense(DateTime date, 
            string campaignId,
            string description = "no description") : this()
        {
            AddedAt = date;
            CampaignId = campaignId;
            Description = description;
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
            CampaignId = campaignId;
        }

        public virtual decimal TotalAmount() => _expenseItems.Sum(item => item.GetTotalPrice());
    }
}
