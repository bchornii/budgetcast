using BudgetCast.Expenses.Commands.Tags;
using BudgetCast.Expenses.Domain.Expenses;

namespace BudgetCast.Expenses.Commands.Expenses
{
    public static class Mapper
    {
        public static Tag[] MapFrom(TagDto[] dto)
        {
            return dto.Select(x => new Tag { Name = x.Name }).ToArray();
        }
    }
}
