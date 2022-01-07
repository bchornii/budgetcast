using AutoFixture;
using BudgetCast.Common.Domain;
using BudgetCast.Common.Tests.Extensions;
using BudgetCast.Expenses.Commands.Expenses;
using BudgetCast.Expenses.Domain.Campaigns;
using BudgetCast.Expenses.Domain.Expenses;
using FluentAssertions;
using Moq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BudgetCast.Expenses.Tests.Unit.Application.Expenses
{
    public class AddExpenseCommandHandlerTests
    {
        private readonly AddExpenseCommandHandlerFixture _fixture;

        public AddExpenseCommandHandlerTests()
        {
            _fixture = new AddExpenseCommandHandlerFixture();
        }

        [Fact]
        public async Task Handle_Newly_Created_Expense_Should_Have_Correct_Data()
        {
            // Arrange
            var command = _fixture.Fixture.Create<AddExpenseCommand>();

            // Act
            await _fixture.Handler.Handle(command, CancellationToken.None);

            // Assert
            var savedExpense = Mock.Get(_fixture.ExpensesRepository)
                .GetExecutionArgumentsOf(nameof(IExpensesRepository.Add))
                .FirstArgumentOf<Expense>();

            savedExpense.ExpenseItems.Count.Should().Be(1);
            savedExpense.ExpenseItems.First().GetTotalPrice().Should().Be(command.TotalAmount);
            savedExpense.Tags.Count.Should().Be(command.Tags.Length);
        }

        [Fact]
        public async Task Handle_Should_Commit_Newly_Created_Expense()
        {
            // Arrange
            var command = _fixture.Fixture.Create<AddExpenseCommand>();

            // Act
            await _fixture.Handler.Handle(command, CancellationToken.None);

            // Assert
            Mock.Get(_fixture.UnitOfWork)
                .Verify(v => v.Commit(), Times.Once());
        }

        private sealed class AddExpenseCommandHandlerFixture
        {
            public Fixture Fixture { get; }

            public IExpensesRepository ExpensesRepository { get; }

            public ICampaignRepository CampaignRepository { get; }

            public IUnitOfWork UnitOfWork { get; }

            public AddExpenseCommandHandler Handler { get; }

            public AddExpenseCommandHandlerFixture()
            {
                Fixture = new Fixture();
                ExpensesRepository = Mock.Of<IExpensesRepository>();
                CampaignRepository = Mock.Of<ICampaignRepository>();
                UnitOfWork = Mock.Of<IUnitOfWork>();
                Handler = new AddExpenseCommandHandler(ExpensesRepository, CampaignRepository, UnitOfWork);
            }
        }
    }
}
