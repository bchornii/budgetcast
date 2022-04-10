using System.Threading;
using System.Threading.Tasks;
using BudgetCast.Common.Operations;
using BudgetCast.Common.Web.Contextual;
using BudgetCast.Common.Web.Messaging;
using BudgetCast.Common.Web.Tests.Messaging.Fakes;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BudgetCast.Common.Web.Tests.Messaging;

public class ExtractOperationContextFromMessageMetadataStepTests
{
    private ExtractOperationContextFromMessageMetadataStepFixture _fixture;

    public ExtractOperationContextFromMessageMetadataStepTests()
    {
        _fixture = new ExtractOperationContextFromMessageMetadataStepFixture();
    }

    [Fact]
    public async Task Execute_WorkloadCtx_Has_OperationContext_Should_Skip_Updating_WorkloadCtx()
    {
        // Arrange
        _fixture
            .SetupWorkloadContextToContainOperationCtx().And()
            .SetupIntegrationMessageToContainOperationCtx();
        

        // Act
        await _fixture.Step.Execute(_fixture.IntegrationMessage, CancellationToken.None);

        // Arrange
        _fixture
            .FakeOperationContext
            .IsEmpty
            .Should()
            .Be(true);
    }

    [Fact]
    public async Task Execute_Message_Does_Not_Contain_Operation_Context_Should_Skip_Updating_WorkloadCtx()
    {
        // Arrange
        _fixture
            .SetupWorkloadContextToBeEmpty().And()
            .SetupIntegrationMessageToContainNoOperationCtx();
        

        // Act
        await _fixture.Step.Execute(_fixture.IntegrationMessage, CancellationToken.None);

        // Arrange
        _fixture
            .FakeOperationContext
            .IsEmpty
            .Should()
            .Be(true);
    }

    /// <summary>
    /// TC1 =
    ///     - Workload Context does not have Operation Context attached
    ///     - Incoming message has Operation Context attached in metadata 
    /// </summary>
    [Fact]
    public async Task Execute_TC1_Should_Update_Attached_To_Message_OpxCtx_With_New_Part_And_Save_It_Into_WorkloadContext()
    {
        // Arrange
        _fixture
            .SetupWorkloadContextToBeEmpty()
            .SetupIntegrationMessageToContainOperationCtx();

        // Act
        await _fixture.Step.Execute(_fixture.IntegrationMessage, CancellationToken.None);

        // Arrange
        var opCtxFromWorkloadCtx = (OperationContext)_fixture.WorkloadContext
            .GetItem(OperationContext.MetaName);

        opCtxFromWorkloadCtx
            .GetDescription()
            .Should()
            .Contain($"{nameof(FakeIntegrationMessage)}_Handler");
    }

    private class ExtractOperationContextFromMessageMetadataStepFixture
    {
        public FakeIntegrationMessage IntegrationMessage { get; }
        
        public OperationContext FakeOperationContext { get; }
        
        public WorkloadContext WorkloadContext { get; }
        
        public ILogger<ExtractOperationContextFromMessageMetadataStep> Logger { get; }
        
        public ExtractOperationContextFromMessageMetadataStep Step { get; }

        public ExtractOperationContextFromMessageMetadataStepFixture()
        {
            FakeOperationContext = OperationContext.New();
            WorkloadContext = new WorkloadContext();
            IntegrationMessage = new FakeIntegrationMessage();
            
            Logger = Mock.Of<ILogger<ExtractOperationContextFromMessageMetadataStep>>();
            Step = new ExtractOperationContextFromMessageMetadataStep(WorkloadContext, Logger);
        }

        public ExtractOperationContextFromMessageMetadataStepFixture SetupWorkloadContextToBeEmpty()
        {
            WorkloadContext.Clear();
            return this;
        }

        public ExtractOperationContextFromMessageMetadataStepFixture SetupWorkloadContextToContainOperationCtx()
        {
            WorkloadContext.AddItem(OperationContext.MetaName, FakeOperationContext);
            return this;
        }

        public ExtractOperationContextFromMessageMetadataStepFixture SetupIntegrationMessageToContainOperationCtx()
        {
            IntegrationMessage.SetMetadata(OperationContext.MetaName, FakeOperationContext.Pack());
            return this;
        }
        
        public ExtractOperationContextFromMessageMetadataStepFixture SetupIntegrationMessageToContainNoOperationCtx()
        {
            IntegrationMessage.Metadata.Clear();
            return this;
        }

        public ExtractOperationContextFromMessageMetadataStepFixture And()
            => this;
    }
}