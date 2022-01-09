using AutoFixture;
using BudgetCast.Expenses.Commands.Expenses;
using BudgetCast.Expenses.Commands.Tags;
using FluentValidation.TestHelper;
using System;
using Xunit;

namespace BudgetCast.Expenses.Tests.Unit.Application.Expenses
{
    public class AddExpenseCommandValidatorTests
    {
        private AddExpenseCommandValidatorFixture _fixture;

        public AddExpenseCommandValidatorTests()
        {
            _fixture = new AddExpenseCommandValidatorFixture();
        }

        [Fact]
        public void AddedAt_HasDefaultValue_Is_Invalid()
        {
            // Arrange
            var defaultCommand = _fixture.CreatePopulated();
            var command = defaultCommand with { AddedAt = default };
            // Act
            var result = _fixture.Validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.AddedAt);
        }

        [Fact]
        public void CampaignName_HasDefaultValue_Is_Invalid()
        {
            // Arrange
            var defaultCommand = _fixture.CreatePopulated();
            var command = defaultCommand with { CampaignName = default };
            // Act
            var result = _fixture.Validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.CampaignName);
        }

        private class AddExpenseCommandValidatorFixture
        {
            public AddExpenseCommandValidator Validator { get; }

            public AddExpenseCommandValidatorFixture()
            {
                Validator = new AddExpenseCommandValidator();
            }

            public AddExpenseCommand CreatePopulated()
                => new Fixture().Create<AddExpenseCommand>();
        }
    }

    public class UpdateExpenseTagsCommandValidatorTests
    {
        private UpdateExpenseTagsCommandValidatorFixture _fixture;

        public UpdateExpenseTagsCommandValidatorTests()
        {
            _fixture = new UpdateExpenseTagsCommandValidatorFixture();
        }

        [Fact]
        public void ExpenseId_IsDefault_IsInvalid()
        {
            // Arrange
            var defaultCommand = _fixture.GetPopulated();
            var command = defaultCommand with { ExpenseId = default };

            // Act
            var result = _fixture.Validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ExpenseId);
        }

        [Fact]
        public void Tags_IsEmptyCollection_IsInvalid()
        {
            // Arrange
            var defaultCommand = _fixture.GetPopulated();
            var command = defaultCommand with { Tags = Array.Empty<TagDto>() };

            // Act
            var result = _fixture.Validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Tags);
        }

        [Fact]
        public void Tags_IsNull_IsInvalid()
        {
            // Arrange
            var defaultCommand = _fixture.GetPopulated();
            var command = defaultCommand with { Tags = default! };

            // Act
            var result = _fixture.Validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Tags);
        }

        private class UpdateExpenseTagsCommandValidatorFixture
        {
            public UpdateExpenseTagsCommandValidator Validator { get; }

            public UpdateExpenseTagsCommandValidatorFixture()
            {
                Validator = new UpdateExpenseTagsCommandValidator();
            }

            public UpdateExpenseTagsCommand GetPopulated()
                => new Fixture().Create<UpdateExpenseTagsCommand>();
        }
    }
}
