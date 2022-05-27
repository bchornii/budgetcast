using BudgetCast.Expenses.Domain.Expenses;

namespace BudgetCast.Expenses.Commands.Expenses
{
    public static class Mapper
    {
        public static Tag[] MapFrom(string[] dto)
        {
            return dto.Select(x => Tag.Create(name: x).Value).ToArray();
        }
    }
}
