using AutoFixture;
using BudgetCast.Common.Tests.Extensions;
using BudgetCast.Expenses.Domain.Campaigns;
using BudgetCast.Expenses.Domain.Expenses;
using FluentAssertions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BudgetCast.Common.Domain;
using BudgetCast.Common.Domain.Results;
using Moq;
using Xunit;

namespace BudgetCast.Expenses.Tests.Unit.Domain.Expenses
{
    public class ExpensesTests
    {
        private readonly ExpenseFixture _fixture;

        public ExpensesTests()
        {
            _fixture = new ExpenseFixture()
                .InitializeDefaultStubs();
        }

        [Fact]
        public async Task TotalAmount_Should_Return_TotalPrice_Of_All_Items()
        {
            // Arrange
            var expense = _fixture.CreateFakeExpense();
            var expenseItems = _fixture.CreateFakeExpenseItems();
            foreach (var item in expenseItems)
            {
                await expense.AddItemAsync(item, _fixture.BusinessRuleRegistry, CancellationToken.None);
            }

            var expectedResult = expenseItems.Sum(ei => ei.GetTotalPrice());

            // Act
            var totalPrice = expense.GetTotalAmount();

            // Assert
            totalPrice.Should().Be(expectedResult);
        }

        [Fact]
        public async Task AddItem_Should_Add_ExpenseItem_To_ExpenseItems_Collection()
        {
            // Arrange
            var expense = _fixture.CreateFakeExpense();
            var expenseItems = _fixture.CreateFakeExpenseItems();

            var initTotalItems = expense.ExpenseItems.Count;

            // Act
            foreach (var item in expenseItems)
            {
                await expense.AddItemAsync(item, _fixture.BusinessRuleRegistry, CancellationToken.None);
            }
            var finalTotalItems = expense.ExpenseItems.Count;

            // Assert
            initTotalItems.Should().Be(0);
            finalTotalItems.Should().Be(expenseItems.Length);
        }

        [Fact]
        public async Task AddItem_100_Items_Exist_Should_Return_Error_Result()
        {
            // Arrange
            var expense = _fixture.CreateFakeExpense();
            var expenseItems = _fixture.CreateFakeExpenseItems(100);
            foreach (var item in expenseItems)
            {
                await expense.AddItemAsync(item, _fixture.BusinessRuleRegistry, CancellationToken.None);
            }

            var initTotalItems = expense.ExpenseItems.Count;

            var expenseItem = _fixture.CreateFakeExpenseItems(1).First();

            // Act
            var result = await expense.AddItemAsync(expenseItem, _fixture.BusinessRuleRegistry, CancellationToken.None);

            // Assert
            initTotalItems.Should().Be(100);
            Assert.False(result);
        }

        [Fact]
        public async Task AddItem_Expense_IsNot_Approved_Should_Return_Error_Result()
        {
            // Arrange
            var expense = _fixture.CreateFakeExpense();
            var expenseItem = _fixture.CreateFakeExpenseItems().First();

            var failedBusinessRule = Mock.Of<IBusinessRule>();
            Mock.Get(failedBusinessRule)
                .Setup(s => s.ValidateAsync(CancellationToken.None))
                .ReturnsAsync(Result.GeneralFail());
            Mock.Get(_fixture.BusinessRuleRegistry)
                .Setup(s => s.Locate(It.IsAny<Type>()))
                .Returns(failedBusinessRule);
            
            // Act
            var result = await expense.AddItemAsync(expenseItem, _fixture.BusinessRuleRegistry, CancellationToken.None);
            
            // Assert
            result.Should().BeOfType<GeneralFail>();
        }
        
        [Fact]
        public void SetCampaignId_Should_Set_Expense_CampaignId()
        {
            // Arrange
            var expense = _fixture.CreateFakeExpense();
            var campaignId = _fixture.Fixture.Create<long>();

            // Act
            expense.SetCampaignId(campaignId);

            // Assert
            expense.GetCampaignId().Should().Be(campaignId);
        }

        [Fact]
        public void AddTags_Adding_More_Than_100_Should_Throw_Error()
        {
            // Arrange
            var expense = _fixture.CreateFakeExpense();
            var tags = _fixture.CreateFakeTags(101);

            var initTagsAmount = expense.Tags.Count;

            // Act
            var result = expense.AddTags(tags);

            // Assert
            initTagsAmount.Should().Be(0);
            Assert.False(result);
        }

        [Fact]
        public void AddTags_Adds_Only_Distinct_Tags()
        {
            // Arrange
            var expense = _fixture.CreateFakeExpense();
            var initialTags = _fixture.CreateFakeTags(5);
            expense.AddTags(initialTags);
            var initTagsAmount = expense.Tags.Count;

            var copyOfInitialTags = initialTags.Select(t => t.Clone()).ToArray();

            var additionalTags = copyOfInitialTags
                .Concat(_fixture.CreateFakeTags(2)).ToArray();

            // Act
            expense.AddTags(additionalTags);

            // Assert
            initTagsAmount.Should().Be(5);
            expense.Tags.Count.Should().Be(7);
        }

        private sealed class ExpenseFixture
        {
            public Fixture Fixture { get; }
            
            public IBusinessRuleRegistry BusinessRuleRegistry { get; }

            public ExpenseFixture()
            {
                Fixture = new Fixture();
                BusinessRuleRegistry = Mock.Of<IBusinessRuleRegistry>();
            }

            public ExpenseFixture InitializeDefaultStubs()
            {
                var businessRule = Mock.Of<IBusinessRule>();
                Mock.Get(businessRule)
                    .Setup(s => s.ValidateAsync(CancellationToken.None))
                    .ReturnsAsync(Success.Empty);
                Mock.Get(BusinessRuleRegistry)
                    .Setup(s => s.Locate(It.IsAny<Type>()))
                    .Returns(businessRule);

                return this;
            }

            public Expense CreateFakeExpense()
            {
                var campaign = Campaign.Create(Fixture.Create<string>()).Value;
                return Expense.Create(DateTime.Today, campaign, Fixture.Create<string>()).Value;
            }

            public ExpenseItem[] CreateFakeExpenseItems(int totalItems = 5)
            {
                var results = new ExpenseItem[totalItems];
                for (int i = 0; i < results.Length; i++)
                {
                    results[i] = new ExpenseItem(
                        title: Fixture.Create<string>(),
                        price: Fixture.Create<decimal>(),
                        quantity: Fixture.Create<int>());
                }
                return results;
            }

            public Tag[] CreateFakeTags(int totalItems = 5)
            {
                var result = new Tag[totalItems];
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = Tag.Create(Fixture.Create<string>()).Value;
                }
                return result;
            }
        }
    }
}
