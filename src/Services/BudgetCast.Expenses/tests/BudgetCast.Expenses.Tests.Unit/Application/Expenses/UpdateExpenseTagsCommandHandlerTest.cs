using AutoFixture;
using BudgetCast.Common.Domain;
using BudgetCast.Common.Tests.Extensions;
using BudgetCast.Expenses.Commands.Tags;
using BudgetCast.Expenses.Domain.Expenses;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BudgetCast.Expenses.Tests.Unit.Application.Expenses
{
    public class UpdateExpenseTagsCommandHandlerTest
    {
        private readonly UpdateExpenseTagsCommandHandlerFixture _fixture;

        public UpdateExpenseTagsCommandHandlerTest()
        {
            _fixture = new UpdateExpenseTagsCommandHandlerFixture();
        }

        [Fact]
        public async Task Handle_Should_Add_Tags_To_Correct_Expense()
        {
            // Arrange
            var command = _fixture.Fixture.Create<UpdateExpenseTagsCommand>();
            var expensesMock = new Mock<Expense>();

            Mock.Get(_fixture.ExpensesRepository)
                .Setup(s => s.GetAsync(It.IsAny<ulong>()))
                .ReturnsAsync(expensesMock.Object);

            // Act
            await _fixture.Handler.Handle(command, CancellationToken.None);

            // Assert
            var tags = Mock.Get(expensesMock.Object)
                .GetExecutionArgumentsOf(nameof(Expense.AddTags))
                .FirstArgumentOf<Tag[]>();

            tags.Should().BeEquivalentTo(command.Tags);
        }

        [Fact]
        public async Task Handle_Should_Commit_Updated_Expense()
        {
            // Arrange
            var command = _fixture.Fixture.Create<UpdateExpenseTagsCommand>();
            var expensesMock = new Mock<Expense>();

            Mock.Get(_fixture.ExpensesRepository)
                .Setup(s => s.GetAsync(It.IsAny<ulong>()))
                .ReturnsAsync(expensesMock.Object);

            // Act
            await _fixture.Handler.Handle(command, CancellationToken.None);

            // Assert
            Mock.Get(_fixture.UnitOfWork)
                .Verify(v => v.Commit(), Times.Once());
        }
        private sealed class UpdateExpenseTagsCommandHandlerFixture
        {
            public Fixture Fixture { get; }

            public IExpensesRepository ExpensesRepository { get; }

            public IUnitOfWork UnitOfWork { get; }

            public UpdateExpenseTagsCommandHandler Handler { get; }

            public UpdateExpenseTagsCommandHandlerFixture()
            {
                Fixture = new Fixture();
                ExpensesRepository = Mock.Of<IExpensesRepository>();
                UnitOfWork = Mock.Of<IUnitOfWork>();
                Handler = new UpdateExpenseTagsCommandHandler(ExpensesRepository, UnitOfWork);
            }

            public Expense CreateFakeExpense()
                => new(Fixture.Create<DateTime>(), Fixture.Create<string>(), Fixture.Create<string>());
        }
    }
}
