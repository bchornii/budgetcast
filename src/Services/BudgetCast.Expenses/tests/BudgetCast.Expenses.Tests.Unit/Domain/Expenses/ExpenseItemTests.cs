using AutoFixture;
using BudgetCast.Expenses.Domain.Expenses;
using FluentAssertions;
using System;
using Xunit;

namespace BudgetCast.Expenses.Tests.Unit.Domain.Expenses
{
    public class ExpenseItemTests
    {
        private readonly ExpenseItemFixture _fixture;

        public ExpenseItemTests()
        {
            _fixture = new ExpenseItemFixture();
        }

        [Fact]
        public void Creation_Of_ExpenseItem_With_Price_Lower_Zero_Throws_Error()
        {
            // Arrange
            Action createExpenseItem = () =>
                new ExpenseItem(
                    title: _fixture.TestTitle,
                    price: -1,
                    quantity: _fixture.TestQuantity);

            // Act

            // Assert
            Assert.Throws<Exception>(createExpenseItem);
        }

        [Fact]
        public void Creation_Of_ExpenseItem_With_Quantity_LessThan_1_Throws_Error()
        {
            // Arrange
            Action createExpenseItem = () =>
                new ExpenseItem(
                    title: _fixture.TestTitle,
                    price: _fixture.TestPrice,
                    quantity: -1);

            // Act

            // Assert
            Assert.Throws<Exception>(createExpenseItem);
        }

        [Fact]
        public void Creation_Of_ExpenseItem_With_Quantity_MoreThan_1000_Throws_Error()
        {
            // Arrange
            Action createExpenseItem = () =>
                new ExpenseItem(
                    title: _fixture.TestTitle,
                    price: _fixture.TestPrice,
                    quantity: 1001);

            // Act

            // Assert
            Assert.Throws<Exception>(createExpenseItem);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Creation_Of_ExpenseItem_With_Empty_Title_Throws_Error(string title)
        {
            // Arrange
            Action createExpenseItem = () =>
                new ExpenseItem(
                    title: title,
                    price: _fixture.TestPrice,
                    quantity: _fixture.TestQuantity);

            // Act

            // Assert
            Assert.Throws<Exception>(createExpenseItem);
        }

        [Fact]
        public void GetTitle_Should_Return_Title()
        {
            // Arrange
            var title = _fixture.Fixture.Create<string>();
            var expenseItem = new ExpenseItem(title, _fixture.TestPrice, _fixture.TestQuantity);

            // Act
            var result = expenseItem.GetTitle();

            // Assert
            result.Should().Be(title);
        }

        [Fact]
        public void GetNote_Should_Return_Note()
        {
            // Arrange
            var note = _fixture.Fixture.Create<string>();
            var expenseItem = new ExpenseItem(
                _fixture.TestTitle,
                _fixture.TestPrice,
                _fixture.TestQuantity,
                note);

            // Act
            var result = expenseItem.GetNote();

            // Assert
            result.Should().Be(note);
        }

        [Fact]
        public void GetPrice_Should_Return_Price()
        {
            // Arrange
            var price = _fixture.Fixture.Create<decimal>();
            var expenseItem = new ExpenseItem(
                _fixture.TestTitle,
                price,
                _fixture.TestQuantity);

            // Act
            var result = expenseItem.GetPrice();

            // Assert
            result.Should().Be(price);
        }

        [Fact]
        public void GetTotalPrice_Should_Return_Correct_TotalPrice()
        {
            // Arrange
            var price = _fixture.Fixture.Create<decimal>();
            var quantity = _fixture.Fixture.Create<int>();
            var totalPrice = price * quantity;

            var expenseItem = new ExpenseItem(
                _fixture.TestTitle,
                price,
                quantity);

            // Act
            var result = expenseItem.GetTotalPrice();

            // Assert
            result.Should().Be(totalPrice);
        }
        private sealed class ExpenseItemFixture
        {
            public Fixture Fixture { get; }

            public string TestTitle => Fixture.Create<string>();

            public decimal TestPrice => 105.0m;

            public int TestQuantity => 12;

            public ExpenseItemFixture()
            {
                Fixture = new Fixture();
            }
        }
    }
}
