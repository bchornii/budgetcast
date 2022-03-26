using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using BudgetCast.Common.Authentication;
using BudgetCast.Common.Web.Messaging;
using BudgetCast.Common.Web.Tests.Messaging.Fakes;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using static BudgetCast.Common.Web.Messaging.MessageMetadataConstants;

namespace BudgetCast.Common.Web.Tests.Messaging;

public class AddUserToMessageMetadataStepTests
{
    private readonly AddUserToMessageMetadataStepFixture _fixture;

    public AddUserToMessageMetadataStepTests()
    {
        _fixture = new AddUserToMessageMetadataStepFixture();
    }
    
    [Fact]
    public async Task Execute_IdentityCtx_DoesNot_Have_User_Should_Not_Set_It_On_Message()
    {
        // Arrange
        var integrationMessage = new FakeIntegrationMessage();
        Mock.Get(_fixture.IdentityContext)
            .Setup(s => s.HasAssociatedUser)
            .Returns(false);

        // Act
        await _fixture.Step.Execute(integrationMessage, CancellationToken.None);

        // Assert
        var result = integrationMessage
            .HasMetadata(UserIdMetadataKey);

        result.Should().Be(false);
    }
    
    [Fact]
    public async Task Execute_IdentityCtx_Has_User_Should_Set_It_On_Message()
    {
        // Arrange
        var integrationMessage = new FakeIntegrationMessage();

        var expectedUserId = _fixture.Fixture.Create<string>();
        Mock.Get(_fixture.IdentityContext)
            .Setup(s => s.HasAssociatedUser)
            .Returns(true);

        Mock.Get(_fixture.IdentityContext)
            .Setup(s => s.UserId)
            .Returns(expectedUserId);

        // Act
        await _fixture.Step.Execute(integrationMessage, CancellationToken.None);

        // Assert
        var result = integrationMessage
            .GetMetadata(UserIdMetadataKey);

        result
            .Should()
            .Be(expectedUserId);
    }
    
    [Fact]
    public async Task Execute_IdentityCtx_And_Message_Both_Have_Tenant_Should_Not_Update_Message()
    {
        // Arrange
        var integrationMessage = new FakeIntegrationMessage();
        var messageUserId = _fixture.Fixture.Create<string>();
        integrationMessage.SetMetadata(UserIdMetadataKey, messageUserId);

        var ctxUserId = _fixture.Fixture.Create<string>();
        Mock.Get(_fixture.IdentityContext)
            .Setup(s => s.HasAssociatedUser)
            .Returns(true);

        Mock.Get(_fixture.IdentityContext)
            .Setup(s => s.UserId)
            .Returns(ctxUserId);

        // Act
        await _fixture.Step.Execute(integrationMessage, CancellationToken.None);

        // Assert
        var result = integrationMessage
            .GetMetadata(UserIdMetadataKey);

        result
            .Should()
            .Contain(messageUserId);
    }
    
    private class AddUserToMessageMetadataStepFixture
    {
        public Fixture Fixture { get; }
        
        public IIdentityContext IdentityContext { get; }
        
        public ILogger<AddUserToMessageMetadataStep> Logger { get; }
        
        public AddUserToMessageMetadataStep Step { get; }

        public AddUserToMessageMetadataStepFixture()
        {
            Fixture = new Fixture();
            IdentityContext = Mock.Of<IIdentityContext>();
            Logger = Mock.Of<ILogger<AddUserToMessageMetadataStep>>();
            Step = new AddUserToMessageMetadataStep(IdentityContext, Logger);
        }
    }
}