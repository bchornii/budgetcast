using BudgetCast.Common.Web.Contextual;
using FluentAssertions;
using Xunit;

namespace BudgetCast.Common.Web.Tests.Contextual;

public class WorkloadContextTests
{
    private WorkloadContextFixture _fixture;

    public WorkloadContextTests()
    {
        _fixture = new WorkloadContextFixture();
    }

    [Fact]
    public void Contains_Items_Is_In_WorkloadContext_ShouldReturn_True()
    {
        // Arrange
        _fixture
            .SetupContextWithFakeKeyValue();

        // Act
        var contains = _fixture.WorkloadContext.Contains(WorkloadContextFixture.FakeKey);

        // Assert
        contains
            .Should()
            .Be(true);
    }
    
    [Fact]
    public void Contains_Items_IsNot_In_WorkloadContext_ShouldReturn_False()
    {
        // Arrange
        _fixture
            .SetupContextAsEmpty();

        // Act
        var contains = _fixture.WorkloadContext.Contains(WorkloadContextFixture.FakeKey);

        // Assert
        contains
            .Should()
            .Be(false);
    }

    [Fact]
    public void AddItem_Should_Set_Single_Item_In_Collection_Event_If_AddItem_Called_SeveralTimes()
    {
        // Arrange
        _fixture
            .SetupContextWithFakeKeyValue();

        // Act
        _fixture
            .SetupContextWithFakeKeyValue();

        // Assert
        _fixture.WorkloadContext
            .TotalItems
            .Should()
            .Be(1);
    }

    [Fact]
    public void GetItem_Should_Return_AddedItem()
    {
        // Arrange
        _fixture
            .SetupContextWithFakeKeyValue();

        // Act
        var result = _fixture.WorkloadContext.GetItem(WorkloadContextFixture.FakeKey);

        // Assert
        result
            .Should()
            .Be(WorkloadContextFixture.FakeValue);
    }

    [Fact]
    public void Clear_Should_Remove_All_Items()
    {
        // Arrange
        _fixture
            .SetupContextWithFakeKeyValue();

        // Act
        _fixture.WorkloadContext.Clear();

        // Assert
        _fixture.WorkloadContext
            .TotalItems
            .Should()
            .Be(0);
    }

    [Fact]
    public void ToString_Should_Return_Correctly_Formatted_Items()
    {
        // Arrange
        _fixture
            .SetupContextWithFakeKeyValue();

        // Act
        var result = _fixture.WorkloadContext.ToString();

        // Assert
        result
            .Should()
            .Be($"{WorkloadContextFixture.FakeKey}:{WorkloadContextFixture.FakeValue}");
    }

    [Fact]
    public void Equals_Should_Return_True_For_The_Same_Context()
    {
        // Arrange
        _fixture
            .SetupContextWithFakeKeyValue();

        // Act
        var result = _fixture.WorkloadContext.Equals(_fixture.WorkloadContext);

        // Assert
        result
            .Should()
            .Be(true);
    }
    
    private class WorkloadContextFixture
    {
        public const string FakeKey = nameof(FakeKey);
        public const string FakeValue = nameof(FakeValue);
        
        public WorkloadContext WorkloadContext { get; }

        public WorkloadContextFixture()
        {
            WorkloadContext = new WorkloadContext();
        }

        public WorkloadContextFixture SetupContextAsEmpty()
        {
            WorkloadContext.Clear();
            return this;
        }

        public WorkloadContextFixture SetupContextWithFakeKeyValue()
        {
            WorkloadContext.AddItem(FakeKey, FakeValue);
            return this;
        }
    }
}