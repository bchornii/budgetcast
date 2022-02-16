using BudgetCast.Common.Domain;

namespace BudgetCast.Expenses.Domain.Expenses
{
    public class Tag : ValueObject
    {
        public string Name { get; init; }

        public Tag()
        {
            Name = default!;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
        }
    }
}
