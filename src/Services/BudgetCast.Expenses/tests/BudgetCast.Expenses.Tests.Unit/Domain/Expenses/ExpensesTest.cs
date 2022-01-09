using AutoFixture;
using BudgetCast.Common.Tests.Extensions;
using BudgetCast.Expenses.Domain.Campaigns;
using BudgetCast.Expenses.Domain.Expenses;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace BudgetCast.Expenses.Tests.Unit.Domain.Expenses
{
    public class ExpensesTests
    {
        private readonly ExpenseFixture _fixture;

        public ExpensesTests()
        {
            _fixture = new ExpenseFixture();
        }

        [Fact]
        public void TotalAmount_Should_Return_TotalPrice_Of_All_Items()
        {
            // Arrange
            var expense = _fixture.CreateFakeExpense();
            var expenseItems = _fixture.CreateFakeExpenseItems();
            foreach (var item in expenseItems)
            {
                expense.AddItem(item);
            }

            var expectedResult = expenseItems.Sum(ei => ei.GetTotalPrice());

            // Act
            var totalPrice = expense.GetTotalAmount();

            // Assert
            totalPrice.Should().Be(expectedResult);
        }

        [Fact]
        public void AddItem_Should_Add_ExpenseItem_To_ExpenseItems_Collection()
        {
            // Arrange
            var expense = _fixture.CreateFakeExpense();
            var expenseItems = _fixture.CreateFakeExpenseItems();

            var initTotalItems = expense.ExpenseItems.Count;

            // Act
            foreach (var item in expenseItems)
            {
                expense.AddItem(item);
            }
            var finalTotalItems = expense.ExpenseItems.Count;

            // Assert
            initTotalItems.Should().Be(0);
            finalTotalItems.Should().Be(expenseItems.Length);
        }

        [Fact]
        public void AddItem_100_Items_Exist_Should_Throw_Error()
        {
            // Arrange
            var expense = _fixture.CreateFakeExpense();
            var expenseItems = _fixture.CreateFakeExpenseItems(100);
            foreach (var item in expenseItems)
            {
                expense.AddItem(item);
            }

            var initTotalItems = expense.ExpenseItems.Count;

            Action addExpenseItem = () =>
            {
                var expenseItem = _fixture.CreateFakeExpenseItems(1).First();
                expense.AddItem(expenseItem);
            };

            // Act

            // Assert
            initTotalItems.Should().Be(100);
            Assert.Throws<Exception>(addExpenseItem);
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
            Action addTags = () =>
            {
                expense.AddTags(tags);
            };

            // Assert
            initTagsAmount.Should().Be(0);
            Assert.Throws<Exception>(addTags);
        }

        [Fact]
        public void AddTags_Adds_Only_Distinct_Tags()
        {
            // Arrange
            var expense = _fixture.CreateFakeExpense();
            var initialTags = _fixture.CreateFakeTags(5);
            expense.AddTags(initialTags);
            var initTagsAmount = expense.Tags.Count;

            var copyOfInitialTags = initialTags.CloneJson();

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

            public ExpenseFixture()
            {
                Fixture = new Fixture();
            }

            public Expense CreateFakeExpense()
                => new(Fixture.Create<DateTime>(), Fixture.Create<Campaign>(), Fixture.Create<string>());

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
                    result[i] = new Tag
                    {
                        Name = Fixture.Create<string>()
                    };
                }
                return result;
            }
        }
    }
}
