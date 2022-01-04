using BudgetCast.Expenses.Domain.Expenses;

namespace BudgetCast.Expenses.Commands.Tags
{
    public static class ExpensesTagsCommandMapper
    {
        public static Tag[] MapFrom(TagDto[] dto)
        {
            return dto.Select(x => new Tag(x.Name, x.ExpenseId)).ToArray();
        }
    }
}
