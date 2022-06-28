using System;
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
using BudgetCast.Common.Authentication;
using BudgetCast.Common.Messaging.Abstractions.Events;
using BudgetCast.Expenses.Commands;
using BudgetCast.Expenses.Messaging;
using Xunit;

namespace BudgetCast.Expenses.Tests.Unit.Application.Expenses
{
    public class AddExpenseCommandHandlerTests
    {
        private readonly AddExpenseCommandHandlerFixture _fixture;

        public AddExpenseCommandHandlerTests()
        {
            _fixture = new AddExpenseCommandHandlerFixture()
                .InitDefaultStubs();
        }

        [Fact]
        public async Task Handle_Campaign_Does_Not_Exist_Should_Create_New_Campaign()
        {
            // Arrange
            var command = _fixture.Fixture.Create<AddExpenseCommand>();

            // Act
            await _fixture.Handler.Handle(command, CancellationToken.None);

            // Assert
            var savedCampaign = Mock.Get(_fixture.CampaignRepository)
                .GetExecutionArgumentsOf(nameof(ICampaignRepository.AddAsync))
                .FirstArgumentOf<Campaign>();

            savedCampaign.Name.Should().Be(command.CampaignName);
        }

        [Fact]
        public async Task Handle_Campaign_Exists_Should_Use_Existing_Campaign_For_New_Expense()
        {
            // Arrange
            var command = _fixture.Fixture.Create<AddExpenseCommand>();
            command = command with {AddedAt = DateTime.Today};
            var campaign = Campaign.Create(_fixture.Fixture.Create<string>()).Value;

            Mock.Get(_fixture.CampaignRepository)
                .Setup(s => s.GetByNameAsync(It.IsAny<string>(), CancellationToken.None))
                .ReturnsAsync(campaign);

            // Act
            await _fixture.Handler.Handle(command, CancellationToken.None);

            // Assert
            var savedExpense = Mock.Get(_fixture.ExpensesRepository)
                .GetExecutionArgumentsOf(nameof(IExpensesRepository.AddAsync))
                .FirstArgumentOf<Expense>();

            savedExpense.GetCampaignId().Should().Be(campaign.Id);
        }

        [Fact]
        public async Task Handle_Newly_Created_Expense_Should_Have_Correct_Data()
        {
            // Arrange
            var command = _fixture.Fixture.Create<AddExpenseCommand>();
            command = command with {AddedAt = DateTime.Today};

            // Act
            await _fixture.Handler.Handle(command, CancellationToken.None);

            // Assert
            var savedExpense = Mock.Get(_fixture.ExpensesRepository)
                .GetExecutionArgumentsOf(nameof(IExpensesRepository.AddAsync))
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
            command = command with {AddedAt = DateTime.Today};

            // Act
            await _fixture.Handler.Handle(command, CancellationToken.None);

            // Assert
            Mock.Get(_fixture.UnitOfWork)
                .Verify(v => v.Commit(CancellationToken.None), Times.Once());
        }

        [Fact]
        public async Task Handle_Should_Add_ExpenseAdded_IntegrationEvent()
        {
            // Arrange
            var tenantId = _fixture.Fixture.Create<long>();
            var expenseId = _fixture.Fixture.Create<long>();
            
            var command = _fixture.Fixture.Create<AddExpenseCommand>();
            
            ExpensesAddedEvent publishedEvent = default!;
            Mock.Get(_fixture.EventLogService)
                .Setup(s => s.AddEventAsync(It.IsAny<ExpensesAddedEvent>(), CancellationToken.None))
                .Callback<IntegrationEvent, CancellationToken>((evt, _) => publishedEvent = (ExpensesAddedEvent)evt);

            Mock.Get(_fixture.IdentityContext)
                .Setup(s => s.TenantId)
                .Returns(tenantId);

            Mock.Get(_fixture.ExpensesRepository)
                .Setup(s => s.AddAsync(It.IsAny<Expense>(), CancellationToken.None))
                .Callback<Expense, CancellationToken>((evt, _) => evt.Id = expenseId);
            
            // Act
            await _fixture.Handler.Handle(command, CancellationToken.None);
            
            // Assert
            Mock.Get(_fixture.EventLogService)
                .Verify(v => v.AddEventAsync(publishedEvent, CancellationToken.None));
        }

        private sealed class AddExpenseCommandHandlerFixture
        {
            public Fixture Fixture { get; }

            public IExpensesRepository ExpensesRepository { get; }

            public ICampaignRepository CampaignRepository { get; }

            public IUnitOfWork UnitOfWork { get; }
            
            public IIntegrationEventLogService EventLogService { get; }
            
            public IIdentityContext IdentityContext { get; }

            public AddExpenseCommandHandler Handler { get; }

            public AddExpenseCommandHandlerFixture()
            {
                Fixture = new Fixture();
                ExpensesRepository = Mock.Of<IExpensesRepository>();
                CampaignRepository = Mock.Of<ICampaignRepository>();
                UnitOfWork = Mock.Of<IUnitOfWork>();
                EventLogService = Mock.Of<IIntegrationEventLogService>();
                IdentityContext = Mock.Of<IIdentityContext>();
                Handler = new AddExpenseCommandHandler(
                    ExpensesRepository, 
                    CampaignRepository, 
                    UnitOfWork,
                    IdentityContext,
                    EventLogService);
            }

            public AddExpenseCommandHandlerFixture InitDefaultStubs()
            {
                var campaign = Campaign.Create(Fixture.Create<string>()).Value;
                Mock.Get(CampaignRepository)
                    .Setup(c => c.AddAsync(It.IsAny<Campaign>(), CancellationToken.None))
                    .ReturnsAsync(campaign);

                var expense = Expense.Create(DateTime.Today, campaign, Fixture.Create<string>()).Value;
                Mock.Get(ExpensesRepository)
                    .Setup(e => e.AddAsync(It.IsAny<Expense>(), CancellationToken.None))
                    .ReturnsAsync(expense);
                
                Mock.Get(IdentityContext)
                    .Setup(s => s.TenantId)
                    .Returns(Fixture.Create<long>());

                return this;
            }
        }
    }
}
