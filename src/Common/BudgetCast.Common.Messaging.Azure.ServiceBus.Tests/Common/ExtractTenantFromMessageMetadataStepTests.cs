using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using BudgetCast.Common.Authentication;
using BudgetCast.Common.Messaging.Abstractions.Common;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Common;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Tests.Common;

public class ExtractTenantFromMessageMetadataStepTests
{
    private ExtractTenantFromMessageMetadataStepFixture _fixture;

    public ExtractTenantFromMessageMetadataStepTests()
    {
        _fixture = new ExtractTenantFromMessageMetadataStepFixture();
    }

    [Fact]
    public async Task Execute_IntegrationMessage_Has_Tenant_Should_Set_It_To_IdentityContext()
    {
        // Arrange
        var expectedTenantId = _fixture.Fixture.Create<long>();
        var integrationMessage = new FakeIntegrationMessage();
        integrationMessage.SetCurrentTenant(expectedTenantId);

        // Act
        await _fixture.Step.Execute(integrationMessage, CancellationToken.None);

        // Assert
        Mock.Get(_fixture.IdentityContext)
            .Verify(v => v.SetCurrentTenant(expectedTenantId));
    }

    [Fact]
    public async Task Execute_IntegrationMessage_Has_Tenant_But_IdentityCtx_Is_Initialized()
    {
        // Arrange
        var messageTenantId = _fixture.Fixture.Create<long>();
        var integrationMessage = new FakeIntegrationMessage();
        integrationMessage.SetCurrentTenant(messageTenantId);

        Mock.Get(_fixture.IdentityContext)
            .Setup(x => x.HasAssociatedTenant)
            .Returns(true);

        // Act
        await _fixture.Step.Execute(integrationMessage, CancellationToken.None);

        // Assert
        Mock.Get(_fixture.IdentityContext)
            .Verify(v => v.SetCurrentTenant(It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async Task Execute_IntegrationMessage_TenantId_IsNull_And_IdentityCtx_Does_Not_Have_Tenant_Should_Not_Set_It()
    {
        // Arrange
        var integrationMessage = new Mock<IntegrationMessage>().Object;

        Mock.Get(integrationMessage)
            .Setup(s => s.GetTenantId())
            .Returns((long?)null);

        // Act
        await _fixture.Step.Execute(integrationMessage, CancellationToken.None);

        // Assert
        Mock.Get(_fixture.IdentityContext)
            .Verify(v => v.SetCurrentTenant(It.IsAny<long>()), Times.Never);
    }
    
    private class ExtractTenantFromMessageMetadataStepFixture
    {
        public Fixture Fixture { get; }
        
        public IIdentityContext IdentityContext { get; }
        
        public ILogger<ExtractTenantFromMessageMetadataStep> Logger { get; }

        public ExtractTenantFromMessageMetadataStep Step { get; }

        public ExtractTenantFromMessageMetadataStepFixture()
        {
            Fixture = new Fixture();
            IdentityContext = Mock.Of<IIdentityContext>();
            Logger = Mock.Of<ILogger<ExtractTenantFromMessageMetadataStep>>();
            Step = new ExtractTenantFromMessageMetadataStep(IdentityContext, Logger);
        }
    }

    private class FakeIntegrationMessage : IntegrationMessage
    {
    }
}