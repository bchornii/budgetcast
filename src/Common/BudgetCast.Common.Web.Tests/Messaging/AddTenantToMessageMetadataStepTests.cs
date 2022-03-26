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

public class AddTenantToMessageMetadataStepTests
{
    private readonly AddTenantToMessageMetadataStepFixture _fixture;

    public AddTenantToMessageMetadataStepTests()
    {
        _fixture = new AddTenantToMessageMetadataStepFixture();
    }
    
    [Fact]
    public async Task Execute_IdentityCtx_DoesNot_Have_Tenant_Should_Not_Set_It_On_Message()
    {
        // Arrange
        var integrationMessage = new FakeIntegrationMessage();
        Mock.Get(_fixture.IdentityContext)
            .Setup(s => s.HasAssociatedTenant)
            .Returns(false);

        // Act
        await _fixture.Step.Execute(integrationMessage, CancellationToken.None);

        // Assert
        var result = integrationMessage
            .HasMetadata(TenantIdMetadataKey);

        result.Should().Be(false);
    }
    
    [Fact]
    public async Task Execute_IdentityCtx_Has_Tenant_Should_Set_It_On_Message()
    {
        // Arrange
        var integrationMessage = new FakeIntegrationMessage();

        var expectedTenantId = _fixture.Fixture.Create<long>();
        Mock.Get(_fixture.IdentityContext)
            .Setup(s => s.HasAssociatedTenant)
            .Returns(true);

        Mock.Get(_fixture.IdentityContext)
            .Setup(s => s.TenantId)
            .Returns(expectedTenantId);

        // Act
        await _fixture.Step.Execute(integrationMessage, CancellationToken.None);

        // Assert
        var result = integrationMessage
            .GetMetadata(TenantIdMetadataKey);

        result
            .Should()
            .Be(expectedTenantId.ToString());
    }
    
    [Fact]
    public async Task Execute_IdentityCtx_And_Message_Both_Have_Tenant_Should_Not_Update_Message()
    {
        // Arrange
        var integrationMessage = new FakeIntegrationMessage();
        var messageTenantId = _fixture.Fixture.Create<long>();
        integrationMessage.SetMetadata(TenantIdMetadataKey, messageTenantId.ToString());

        var ctxTenantId = _fixture.Fixture.Create<long>();
        Mock.Get(_fixture.IdentityContext)
            .Setup(s => s.HasAssociatedTenant)
            .Returns(true);

        Mock.Get(_fixture.IdentityContext)
            .Setup(s => s.TenantId)
            .Returns(ctxTenantId);

        // Act
        await _fixture.Step.Execute(integrationMessage, CancellationToken.None);

        // Assert
        var result = integrationMessage
            .GetMetadata(TenantIdMetadataKey);

        result
            .Should()
            .Contain(messageTenantId.ToString());
    }
    
    private class AddTenantToMessageMetadataStepFixture
    {
        public Fixture Fixture { get; }
        
        public IIdentityContext IdentityContext { get; }
        
        public ILogger<AddTenantToMessageMetadataStep> Logger { get; }
        
        public AddTenantToMessageMetadataStep Step { get; }

        public AddTenantToMessageMetadataStepFixture()
        {
            Fixture = new Fixture();
            IdentityContext = Mock.Of<IIdentityContext>();
            Logger = Mock.Of<ILogger<AddTenantToMessageMetadataStep>>();
            Step = new AddTenantToMessageMetadataStep(IdentityContext, Logger);
        }
    }
}