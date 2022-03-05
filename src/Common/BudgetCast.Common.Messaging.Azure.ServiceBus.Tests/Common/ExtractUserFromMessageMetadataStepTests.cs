using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using BudgetCast.Common.Authentication;
using BudgetCast.Common.Messaging.Abstractions.Common;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Common;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Tests.Common;

public class ExtractUserFromMessageMetadataStepTests
{
    private readonly ExtractUserFromMessageMetadataStepFixture _fixture;

    public ExtractUserFromMessageMetadataStepTests()
    {
        _fixture = new ExtractUserFromMessageMetadataStepFixture();
    }

    [Fact]
    public async Task Execute_IntegrationMessage_Has_User_Should_Set_It_To_IdentityContext()
    {
        // Arrange
        var expectedUserId = _fixture.Fixture.Create<string>();
        var integrationMessage = new FakeIntegrationMessage();
        integrationMessage.SetUserId(expectedUserId);

        // Act
        await _fixture.Step.Execute(integrationMessage, CancellationToken.None);

        // Assert
        Mock.Get(_fixture.IdentityContext)
            .Verify(v => v.SetUserId(expectedUserId));
    }

    [Fact]
    public async Task Execute_IntegrationMessage_Has_User_But_IdentityCtx_Is_Initialized_Should_Not_Update_It()
    {
        // Arrange
        var messageUserId = _fixture.Fixture.Create<string>();
        var integrationMessage = new FakeIntegrationMessage();
        integrationMessage.SetUserId(messageUserId);

        Mock.Get(_fixture.IdentityContext)
            .Setup(x => x.HasAssociatedUser)
            .Returns(true);

        // Act
        await _fixture.Step.Execute(integrationMessage, CancellationToken.None);

        // Assert
        Mock.Get(_fixture.IdentityContext)
            .Verify(v => v.SetUserId(It.IsAny<string>()), Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task Execute_IntegrationMessage_UserId_IsNull_And_IdentityCtx_Does_Not_Have_Tenant_Should_Not_Set_It(
        string messageUserId)
    {
        // Arrange
        var integrationMessage = new Mock<IntegrationMessage>().Object;

        Mock.Get(integrationMessage)
            .Setup(s => s.GetUserId())
            .Returns(messageUserId);

        // Act
        await _fixture.Step.Execute(integrationMessage, CancellationToken.None);

        // Assert
        Mock.Get(_fixture.IdentityContext)
            .Verify(v => v.SetUserId(It.IsAny<string>()), Times.Never);
    }
    
    private class ExtractUserFromMessageMetadataStepFixture
    {
        public Fixture Fixture { get; }
        
        public IIdentityContext IdentityContext { get; }
        
        public ILogger<ExtractUserFromMessageMetadataStep> Logger { get; }

        public ExtractUserFromMessageMetadataStep Step { get; }

        public ExtractUserFromMessageMetadataStepFixture()
        {
            Fixture = new Fixture();
            IdentityContext = Mock.Of<IIdentityContext>();
            Logger = Mock.Of<ILogger<ExtractUserFromMessageMetadataStep>>();
            Step = new ExtractUserFromMessageMetadataStep(IdentityContext, Logger);
        }
    }

    private class FakeIntegrationMessage : IntegrationMessage
    {
    }
}