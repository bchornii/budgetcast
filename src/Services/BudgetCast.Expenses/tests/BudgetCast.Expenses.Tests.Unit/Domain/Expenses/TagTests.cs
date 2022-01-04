using BudgetCast.Expenses.Domain.Expenses;
using FluentAssertions;
using Xunit;

namespace BudgetCast.Expenses.Tests.Unit.Domain.Expenses
{
    public class TagTests
    {
        [Theory]
        [InlineData("Name1", "ExpenseID4")]
        [InlineData("Name2", "ExpenseID3")]
        [InlineData("Name3", "ExpenseID2")]
        [InlineData("Name4", "ExpenseID1")]
        public void TagCtor_DoesNot_Change_Values_During_Init(string name, string expenseId)
        {
            // Arrange
            var tag = new Tag(name, expenseId);

            // Act

            // Assert
            tag.ExpenseId.Should().Be(expenseId);
            tag.Name.Should().Be(name);
        }
    }
}
