using AutoFixture;
using BudgetCast.Common.Application.Behavior.Idempotency;
using BudgetCast.Common.Application.Command;
using BudgetCast.Common.Application.Tests.Unit.Stubs;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BudgetCast.Common.Domain.Results;
using BudgetCast.Common.Operations;
using Xunit;

namespace BudgetCast.Common.Application.Tests.Unit.Idempotency
{
    public class IdempotentBehaviorTests
    {
        private IdempotentBehaviorFixture<ICommand<Result>, Result> _nonGenericResultBehavior;
        private IdempotentBehaviorFixture<ICommand<Result<FakeData>>, Result<FakeData>> _genericResultBehavior;
        public IdempotentBehaviorTests()
        {
            _nonGenericResultBehavior = new IdempotentBehaviorFixture<ICommand<Result>, Result>();
            _genericResultBehavior = new IdempotentBehaviorFixture<ICommand<Result<FakeData>>, Result<FakeData>>();
        }

        #region Verify values of deserialized results of existing in registry operations

        [Fact]
        public async Task Handle_NotGenericResult_And_OperationExists_Should_Return_SuccessEmpty()
        {
            // Arrange
            var successHandler = _nonGenericResultBehavior.StubHandlerDelegate();
            Mock.Get(_nonGenericResultBehavior.OperationsRegistry)
                .Setup(s => s.TryAddCurrentOperationAsync(CancellationToken.None))
                .ReturnsAsync((IsOperationExists: true, OperationResult: string.Empty));

            // Act
            var result = await _nonGenericResultBehavior
                .Behavior
                .Handle(new FakeCommand(), CancellationToken.None, successHandler);

            // Assert
            result.Should().Be(Success.Empty);
        }

        [Theory]
        [MemberData(nameof(GetFakeObjectsAndTheirJson))]
        public async Task Handle_GenericResult_And_OperationExists_Should_Return_Success_Of_Type_FakeData(
            FakeData fakeData,
            string fakeDataAsOperationResult)
        {
            // Arrange
            var successHandler = _genericResultBehavior.StubHandlerDelegate();
            Mock.Get(_genericResultBehavior.OperationsRegistry)
                .Setup(s => s.TryAddCurrentOperationAsync(CancellationToken.None))
                .ReturnsAsync((IsOperationExists: true, OperationResult: fakeDataAsOperationResult));
            
            // Act
            var result = await _genericResultBehavior
                .Behavior
                .Handle(new FakeGenericCommand(), CancellationToken.None, successHandler);

            // Assert
            result
                .Should().BeOfType<Success<FakeData>>()
                .Subject.Value
                .Should().Be(fakeData);
        }

        #endregion

        #region Verify command handler result returned as expected

        [Theory]
        [MemberData(nameof(CommonHelpers.GetResultTypes), MemberType = typeof(CommonHelpers))]
        public async Task Handle_NonGenericResult_Operation_DoesNot_Exist_Should_Execute_Handler_And_Return_It_Result(Result commandResult)
        {
            // Arrange
            var successHandler = _nonGenericResultBehavior.HandlerDelegate(commandResult);
            Mock.Get(_nonGenericResultBehavior.OperationsRegistry)
                .Setup(s => s.TryAddCurrentOperationAsync(CancellationToken.None))
                .ReturnsAsync((IsOperationExists: false, OperationResult: string.Empty));

            // Act
            var result = await _nonGenericResultBehavior
                .Behavior
                .Handle(new FakeCommand(), CancellationToken.None, successHandler);

            // Assert
            result.Should().Be(commandResult);
        }
        
        [Theory]
        [MemberData(nameof(CommonHelpers.GetGenericResultTypes), MemberType = typeof(CommonHelpers))]
        public async Task Handle_GenericResult_Operation_DoesNot_Exist_Should_Execute_Handler_And_Return_It_Result(Result<FakeData> commandResult)
        {
            // Arrange
            var successHandler = _genericResultBehavior.HandlerDelegate(commandResult);
            Mock.Get(_genericResultBehavior.OperationsRegistry)
                .Setup(s => s.TryAddCurrentOperationAsync(CancellationToken.None))
                .ReturnsAsync((IsOperationExists: false, OperationResult: string.Empty));

            // Act
            var result = await _genericResultBehavior
                .Behavior
                .Handle(new FakeGenericCommand(), CancellationToken.None, successHandler);

            // Assert
            result.Should().Be(commandResult);
        }

        #endregion

        #region Verify that non success result not being stored into registry

        [Theory]
        [MemberData(nameof(CommonHelpers.GetFailResultTypes), MemberType = typeof(CommonHelpers))]
        public async Task Handle_NonGeneric_Operation_Does_Not_Exist_Should_Skip_Saving_It_When_Result_Is_Not_Success(Result commandResult)
        {
            // Arrange
            var successHandler = _nonGenericResultBehavior.HandlerDelegate(commandResult);
            Mock.Get(_nonGenericResultBehavior.OperationsRegistry)
                .Setup(s => s.TryAddCurrentOperationAsync(CancellationToken.None))
                .ReturnsAsync((IsOperationExists: false, OperationResult: string.Empty));

            // Act
            await _nonGenericResultBehavior
                .Behavior
                .Handle(new FakeCommand(), CancellationToken.None, successHandler);

            // Assert
            Mock.Get(_nonGenericResultBehavior.OperationsRegistry)
                .Verify(v => v.SetCurrentOperationCompletedAsync((CancellationToken) CancellationToken.None), Times.Never);
            Mock.Get(_nonGenericResultBehavior.OperationsRegistry)
                .Verify(v => v.SetCurrentOperationCompletedAsync(It.IsAny<string>(), CancellationToken.None), Times.Never);
        }

        [Theory]
        [MemberData(nameof(CommonHelpers.GetFailGenericResultTypes), MemberType = typeof(CommonHelpers))]
        public async Task Handle_Generic_Operation_Does_Not_Exist_Should_Skip_Saving_It_When_Result_Is_Not_Success(Result<FakeData> commandResult)
        {
            // Arrange
            var successHandler = _genericResultBehavior.HandlerDelegate(commandResult);
            Mock.Get(_genericResultBehavior.OperationsRegistry)
                .Setup(s => s.TryAddCurrentOperationAsync(CancellationToken.None))
                .ReturnsAsync((IsOperationExists: false, OperationResult: string.Empty));

            // Act
            await _genericResultBehavior
                .Behavior
                .Handle(new FakeGenericCommand(), CancellationToken.None, successHandler);

            // Assert
            Mock.Get(_genericResultBehavior.OperationsRegistry)
                .Verify(v => v.SetCurrentOperationCompletedAsync((CancellationToken) CancellationToken.None), Times.Never);
            Mock.Get(_genericResultBehavior.OperationsRegistry)
                .Verify(v => v.SetCurrentOperationCompletedAsync(It.IsAny<string>(), CancellationToken.None), Times.Never);
        }

        #endregion

        #region Verify that success command result add record into registry

        [Theory]
        [MemberData(nameof(CommonHelpers.GetSuccessResultTypes), MemberType = typeof(CommonHelpers))]
        public async Task Handle_NonGeneric_Operation_Does_Not_Exist_Should_Save_It_When_Result_Is_Success(Result commandResult)
        {
            // Arrange
            var successHandler = _nonGenericResultBehavior.HandlerDelegate(commandResult);
            Mock.Get(_nonGenericResultBehavior.OperationsRegistry)
                .Setup(s => s.TryAddCurrentOperationAsync(CancellationToken.None))
                .ReturnsAsync((IsOperationExists: false, OperationResult: string.Empty));

            // Act
            await _nonGenericResultBehavior
                .Behavior
                .Handle(new FakeCommand(), CancellationToken.None, successHandler);

            // Assert
            Mock.Get(_nonGenericResultBehavior.OperationsRegistry)
                .Verify(v => v.SetCurrentOperationCompletedAsync((CancellationToken) CancellationToken.None), Times.Once);
            Mock.Get(_nonGenericResultBehavior.OperationsRegistry)
                .Verify(v => v.SetCurrentOperationCompletedAsync(It.IsAny<string>(), CancellationToken.None), Times.Never);
        }

        [Theory]
        [MemberData(nameof(CommonHelpers.GetSuccessGenericResultTypes), MemberType = typeof(CommonHelpers))]
        public async Task Handle_Generic_Operation_Does_Not_Exist_Should_Save_It_When_Result_Is_Not_Success(Result<FakeData> commandResult)
        {
            // Arrange
            var successHandler = _genericResultBehavior.HandlerDelegate(commandResult);
            var json = JsonSerializer.Serialize(commandResult, commandResult.GetType(), AppConstants.DefaultOptions);

            Mock.Get(_genericResultBehavior.OperationsRegistry)
                .Setup(s => s.TryAddCurrentOperationAsync(CancellationToken.None))
                .ReturnsAsync((IsOperationExists: false, OperationResult: string.Empty));

            // Act
            await _genericResultBehavior
                .Behavior
                .Handle(new FakeGenericCommand(), CancellationToken.None, successHandler);

            // Assert
            Mock.Get(_genericResultBehavior.OperationsRegistry)
                .Verify(v => v.SetCurrentOperationCompletedAsync((CancellationToken) CancellationToken.None), Times.Never);
            Mock.Get(_genericResultBehavior.OperationsRegistry)
                .Verify(v => v.SetCurrentOperationCompletedAsync(json, CancellationToken.None), Times.Once);
        }
        #endregion

        #region Verify that behavior does not swallow exceptions throwed by command handler
        [Fact]
        public async Task Handle_NonGeneric_CommandHandler_Throws_Should_Propagate_Exception()
        {
            // Arrange
            var successHandler = _nonGenericResultBehavior.ExceptionHandlerDelegate();
            Mock.Get(_nonGenericResultBehavior.OperationsRegistry)
                .Setup(s => s.TryAddCurrentOperationAsync(CancellationToken.None))
                .ReturnsAsync((IsOperationExists: false, OperationResult: string.Empty));
            // Act
            var result = _nonGenericResultBehavior
                .Behavior
                .Handle(new FakeCommand(), CancellationToken.None, successHandler);

            // Assert
            _ = Assert.ThrowsAsync<InvalidOperationException>(async () => await result);
        }

        [Fact]
        public async Task Handle_Generic_CommandHandler_Throws_Should_Propagate_Exception()
        {
            // Arrange
            var successHandler = _genericResultBehavior.ExceptionHandlerDelegate();
            Mock.Get(_genericResultBehavior.OperationsRegistry)
                .Setup(s => s.TryAddCurrentOperationAsync(CancellationToken.None))
                .ReturnsAsync((IsOperationExists: false, OperationResult: string.Empty));
            // Act
            var result = _genericResultBehavior
                .Behavior
                .Handle(new FakeGenericCommand(), CancellationToken.None, successHandler);

            // Assert
            _ = Assert.ThrowsAsync<InvalidOperationException>(async () => await result);
        }
        #endregion
        private class IdempotentBehaviorFixture<TRequest, TResponse>
            where TRequest : ICommand<TResponse>
            where TResponse : Result
        {
            public IOperationsRegistry OperationsRegistry { get; }

            public ILogger<IdempotentBehavior<TRequest, TResponse>> Logger { get; }

            public IdempotentBehavior<TRequest, TResponse> Behavior { get; }

            public IdempotentBehaviorFixture()
            {
                OperationsRegistry = Mock.Of<IOperationsRegistry>();
                Logger = Mock.Of<ILogger<IdempotentBehavior<TRequest, TResponse>>>();
                Behavior = new IdempotentBehavior<TRequest, TResponse>(OperationsRegistry, Logger);
            }
            /// <summary>
            /// Used only in cases when actual delegate execution is skipped.
            /// </summary>
            public RequestHandlerDelegate<TResponse> StubHandlerDelegate()
                => () => Task.FromResult<TResponse>(default);

            /// <summary>
            /// Used to represent commands of non-generic response types such as <see cref="ICommand{TResult}"/>
            /// where <c>TResult</c> is <see cref="Result"/>.
            /// </summary>
            public RequestHandlerDelegate<TResponse> HandlerDelegate(Result result)
                => () => Task.FromResult(result as TResponse);

            /// <summary>
            /// Used to represent commands of non-generic response types such as <see cref="ICommand{TResult}"/>
            /// where <c>TResult</c> is <see cref="Result{T}"/>.
            /// </summary>
            public RequestHandlerDelegate<TResponse> HandlerDelegate(Result<FakeData> result)
                => () => Task.FromResult(result as TResponse);

            /// <summary>
            /// Used to represent command handler which throws an exception.
            /// </summary>
            public RequestHandlerDelegate<TResponse> ExceptionHandlerDelegate()
                => () => throw new InvalidOperationException();

        }
        public static IEnumerable<object[]> GetFakeObjectsAndTheirJson()
        {
            var fixture = new Fixture();
            var fakeObjects = fixture.CreateMany<FakeData>().ToArray();
            foreach (var fakeObject in fakeObjects)
            {
                yield return new object[]
                {
                    fakeObject,
                    $"{{\"Value\": {JsonSerializer.Serialize(fakeObject, typeof(FakeData))}}}",
                };
            }
            yield return new object[]
            {
                new FakeData
                {
                    DealNumber = 123,
                    DwellingAge = 1,
                    Notes = "Some notes",
                },
                "{\"Value\":{\"DealNumber\":123,\"DwellingAge\":1, \"Notes\":\"Some notes\", \"AdditionalProperty\": 123}}",
            };
            yield return new object[]
            {
                new FakeData
                {
                    DealNumber = 123,
                },
                "{\"Value\":{\"DealNumber\":123}}",
            };
        }
    }
}
