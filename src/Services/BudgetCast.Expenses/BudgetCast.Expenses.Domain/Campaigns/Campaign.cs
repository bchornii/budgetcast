using BudgetCast.Common.Domain;

namespace BudgetCast.Expenses.Domain.Campaigns
{
    public class Campaign : AggregateRoot
    {
        public string Name { get; private set; }

        public DateTime StartsAt { get; private set; }

        public DateTime CompletesAt { get; private set; }

        protected Campaign() 
        {
            Name = default!;
        }

        public Campaign(string name) : this()
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception("Campaign should have title.");
            }

            Name = name;

            (StartsAt, CompletesAt) = GetFirstAndLastDaysOfTheMonth();
        }

        public Campaign(string title, DateTime startsAt, DateTime completesAt) : this(title)
        {
            if (startsAt > completesAt)
            {
                throw new Exception("Campaign start date should not be ahead of complete date.");
            }

            StartsAt = startsAt;
            CompletesAt = completesAt;
        }

        private static (DateTime, DateTime) GetFirstAndLastDaysOfTheMonth()
        {
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            return (firstDayOfMonth, lastDayOfMonth);
        }
    }
}
