using AutoFixture;
using BudgetCast.Common.Domain;
using BudgetCast.Expenses.Commands.Campaigns;
using BudgetCast.Expenses.Domain.Campaigns;
using BudgetCast.Expenses.Tests.Unit.Extensions;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BudgetCast.Expenses.Tests.Unit.Application.Campaings
{
    public class CreateMonthlyCampaignCommandHandlerTests
    {
        private CreateMonthlyCampaignCommandHandlerFixture _fixture;

        public CreateMonthlyCampaignCommandHandlerTests()
        {
            _fixture = new CreateMonthlyCampaignCommandHandlerFixture();
        }

        [Fact]
        public async Task Handle_Newly_Created_Campaign_Should_Have_Correct_Data()
        {
            // Arrange
            var command = _fixture.Fixture.Create<CreateMonthlyCampaignCommand>();

            // Act
            await _fixture.Handler.Handle(command, CancellationToken.None);

            // Assert
            var savedCampaign = Mock.Get(_fixture.CampaignRepository)
                .GetExecutionArgumentsOf(nameof(ICampaignRepository.Add))
                .FirstArgumentOf<Campaign>();

            savedCampaign.Name.Should().Be(command.Name);
        }

        [Fact]
        public async Task Handle_Should_Commit_Newly_Created_Campaign()
        {
            // Arrange
            var command = _fixture.Fixture.Create<CreateMonthlyCampaignCommand>();

            // Act
            await _fixture.Handler.Handle(command, CancellationToken.None);

            // Assert
            Mock.Get(_fixture.UnitOfWork)
                .Verify(v => v.Commit(), Times.Once());
        }

        private sealed class CreateMonthlyCampaignCommandHandlerFixture
        {
            public Fixture Fixture { get; }

            public ICampaignRepository CampaignRepository { get; }

            public IUnitOfWork UnitOfWork { get; }

            public CreateMonthlyCampaignCommandHandler Handler { get; }

            public CreateMonthlyCampaignCommandHandlerFixture()
            {
                Fixture = new Fixture();
                CampaignRepository = Mock.Of<ICampaignRepository>();
                UnitOfWork = Mock.Of<IUnitOfWork>();
                Handler = new CreateMonthlyCampaignCommandHandler(CampaignRepository, UnitOfWork);
            }
        }
    }
}
