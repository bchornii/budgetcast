using BudgetCast.Common.Domain;
using BudgetCast.Common.Domain.Results;

namespace BudgetCast.Expenses.Domain.Expenses
{
    public class Tag : ValueObject
    {
        public string Name { get; init; }

        private Tag(string name)
        {
            Name = name;
        }

        public static Result<Tag> Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Errors.Expenses.Tags.TagNameShouldHaveValue();
            }

            return new Tag(name);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
        }
    }
}
