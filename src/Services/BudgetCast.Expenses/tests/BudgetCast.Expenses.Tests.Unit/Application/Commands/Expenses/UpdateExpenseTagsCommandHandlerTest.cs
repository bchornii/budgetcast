using System;
using AutoFixture;
using BudgetCast.Common.Domain;
using BudgetCast.Common.Tests.Extensions;
using BudgetCast.Expenses.Commands.Tags;
using BudgetCast.Expenses.Domain.Expenses;
using FluentAssertions;
using Moq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BudgetCast.Expenses.Domain.Campaigns;
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
            var expense = _fixture.CreateExpenseWithDefaultData();

            Mock.Get(_fixture.ExpensesRepository)
                .Setup(s => s.GetAsync(It.IsAny<long>(), CancellationToken.None))
                .ReturnsAsync(expense);

            // Act
            await _fixture.Handler.Handle(command, CancellationToken.None);

            // Assert
            var expenseTags = expense.Tags.Select(t => t.Name).ToArray();

            expenseTags
                .Should()
                .BeEquivalentTo(command.Tags);
        }

        [Fact]
        public async Task Handle_Should_Commit_Updated_Expense()
        {
            // Arrange
            var command = _fixture.Fixture.Create<UpdateExpenseTagsCommand>();
            var expense = _fixture.CreateExpenseWithDefaultData();

            Mock.Get(_fixture.ExpensesRepository)
                .Setup(s => s.GetAsync(It.IsAny<long>(), CancellationToken.None))
                .ReturnsAsync(expense);

            // Act
            await _fixture.Handler.Handle(command, CancellationToken.None);

            // Assert
            Mock.Get(_fixture.UnitOfWork)
                .Verify(v => v.Commit(CancellationToken.None), Times.Once());
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

            public Expense CreateExpenseWithDefaultData()
            {
                var campaign = Campaign.Create(Fixture.Create<string>()).Value;
                return Expense.Create(
                    Fixture.Create<DateTime>(),
                    campaign,
                    Fixture.Create<string>()).Value;
            }
        }
    }
}
