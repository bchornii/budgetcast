using BudgetCast.Expenses.Domain.Expenses;
using FluentAssertions;
using Xunit;

namespace BudgetCast.Expenses.Tests.Unit.Domain.Expenses
{
    public class TagTests
    {
        [Theory]
        [InlineData("Name1", 1)]
        [InlineData("Name2", 2)]
        [InlineData("Name3", 3)]
        [InlineData("Name4", 4)]
        public void TagCtor_DoesNot_Change_Values_During_Init(string name, ulong expenseId)
        {
            // Arrange
            var tag = new Tag
            {
                Name = name,
                ExpenseId = expenseId
            };

            // Act

            // Assert
            tag.ExpenseId.Should().Be(expenseId);
            tag.Name.Should().Be(name);
        }
    }
}
