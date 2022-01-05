using BudgetCast.Expenses.Domain.Expenses;

namespace BudgetCast.Expenses.Commands.Tags
{
    public static class ExpensesTagsCommandMapper
    {
        public static Tag[] MapFrom(TagDto[] dto)
        {
            return dto.Select(x => new Tag { Name = x.Name, ExpenseId = x.ExpenseId }).ToArray();
        }
    }
}
