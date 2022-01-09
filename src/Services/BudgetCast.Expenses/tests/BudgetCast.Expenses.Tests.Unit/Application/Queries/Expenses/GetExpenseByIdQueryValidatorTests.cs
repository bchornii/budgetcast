using BudgetCast.Expenses.Queries.Expenses.GetExpenseById;
using FluentValidation.TestHelper;
using Xunit;

namespace BudgetCast.Expenses.Tests.Unit.Application.Queries.Expenses
{
    public class GetExpenseByIdQueryValidatorTests
    {
        private GetExpenseByIdQueryValidator _validator;

        public GetExpenseByIdQueryValidatorTests()
        {
            _validator = new GetExpenseByIdQueryValidator();
        }

        [Fact]
        public void ExpenseId_HasDefaultValue_IsInvalid()
        {
            // Arrange

            // Act
            var result = _validator.TestValidate(new GetExpenseByIdQuery(default));

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ExpenseId);
        }
    }
}
