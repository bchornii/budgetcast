using System;
using System.Threading;
using System.Threading.Tasks;
using BudgetCast.Common.Operations;
using BudgetCast.Common.Web.Messaging;
using BudgetCast.Common.Web.Tests.Messaging.Fakes;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BudgetCast.Common.Web.Tests.Messaging;

public class AddOrUpdateOperationContextTests
{
    private AddOrUpdateOperationContextFixture _fixture;

    public AddOrUpdateOperationContextTests()
    {
        _fixture = new AddOrUpdateOperationContextFixture();
    }

    [Fact]
    public async Task Execute_Parent_OperationCtx_Presents_Should_Pass_It_Into_Message_With_It()
    {
        // Arrange
        var fakeMessage = new FakeIntegrationMessage();
        
        _fixture
            .SetupServiceProviderToReturnNonEmptyOperationContext();

        // Act
        await _fixture.Step.Execute(fakeMessage, CancellationToken.None);

        // Assert
        var operationCtx = fakeMessage.GetMetadata(OperationContext.MetaName);

        operationCtx
            .Should()
            .Contain(AddOrUpdateOperationContextFixture.FakeInitialOperationName);
    }

    [Fact]
    public async Task Execute_No_Parent_OperationCtx_But_MessageMetadata_Has_OperationCtx_Should_Do_Nothing()
    {
        // Arrange
        var fakeMessage = new FakeIntegrationMessage();
        var initialMessageOperationCtx = _fixture.FakeExistingOperationContext.Pack();
        fakeMessage.SetMetadata(OperationContext.MetaName, initialMessageOperationCtx);
        
        _fixture
            .SetupServiceProviderToReturnEmptyOperationContext();
        
        // Act
        await _fixture.Step.Execute(fakeMessage, CancellationToken.None);
        
        // Assert
        var operationCtx = fakeMessage.GetMetadata(OperationContext.MetaName);

        operationCtx
            .Should()
            .BeEquivalentTo(initialMessageOperationCtx);
    }
    
    [Fact]
    public async Task Execute_No_Parent_OperationCtx_And_MessageMetadata_Has_No_OperationCtx_Should_Initialize_And_Add_New_OperationCtx()
    {
        var fakeMessage = new FakeIntegrationMessage();
        
        _fixture
            .SetupServiceProviderToReturnEmptyOperationContext();
        
        // Act
        await _fixture.Step.Execute(fakeMessage, CancellationToken.None);
        
        // Assert
        var operationCtx = fakeMessage.GetMetadata(OperationContext.MetaName);
        
        operationCtx
            .Should()
            .Contain(AddOrUpdateOperationContextStep.InitialOperationSuffix);
    }

    private class AddOrUpdateOperationContextFixture
    {
        public const string FakeInitialOperationName = "Fake_Initial_Operation";
        
        private IServiceProvider ServiceProvider { get; }

        private ILogger<AddOrUpdateOperationContextStep> Logger { get; }
        
        public OperationContext FakeExistingOperationContext { get; }

        public AddOrUpdateOperationContextStep Step { get; }

        public AddOrUpdateOperationContextFixture()
        {
            FakeExistingOperationContext = OperationContext.New();
            FakeExistingOperationContext.Add(new OperationPart(FakeInitialOperationName));
            
            ServiceProvider = Mock.Of<IServiceProvider>();
            Logger = Mock.Of<ILogger<AddOrUpdateOperationContextStep>>();
            Step = new AddOrUpdateOperationContextStep(ServiceProvider, Logger);
        }

        public AddOrUpdateOperationContextFixture SetupServiceProviderToReturnEmptyOperationContext()
        {
            Mock.Get(ServiceProvider)
                .Setup(s => s.GetService(typeof(OperationContext)))
                .Returns(OperationContext.New());
            
            return this;
        }
        
        public AddOrUpdateOperationContextFixture SetupServiceProviderToReturnNonEmptyOperationContext()
        {
            Mock.Get(ServiceProvider)
                .Setup(s => s.GetService(typeof(OperationContext)))
                .Returns(FakeExistingOperationContext);
            
            return this;
        }
    }
}