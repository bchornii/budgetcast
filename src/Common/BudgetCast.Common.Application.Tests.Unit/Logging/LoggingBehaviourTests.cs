using BudgetCast.Common.Application.Behavior.Logging;
using BudgetCast.Common.Application.Command;
using BudgetCast.Common.Application.Queries;
using BudgetCast.Common.Application.Tests.Unit.Stubs;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BudgetCast.Common.Application.Tests.Unit.Logging
{
    public class LoggingBehaviourTests
    {
        private LoggingBehaviorFixture<IQuery<Result>, Result> _nonGenericResultQueryBehavior;
        private LoggingBehaviorFixture<IQuery<Result<FakeData>>, Result<FakeData>> _genericResultQueryBehavior;

        private LoggingBehaviorFixture<ICommand<Result>, Result> _nonGenericResultCommandBehavior;
        private LoggingBehaviorFixture<ICommand<Result<FakeData>>, Result<FakeData>> _genericResultCommandBehavior;

        public LoggingBehaviourTests()
        {
            _nonGenericResultQueryBehavior = new LoggingBehaviorFixture<IQuery<Result>, Result>();
            _nonGenericResultCommandBehavior = new LoggingBehaviorFixture<ICommand<Result>, Result>();

            _genericResultQueryBehavior = new LoggingBehaviorFixture<IQuery<Result<FakeData>>, Result<FakeData>>();
            _genericResultCommandBehavior = new LoggingBehaviorFixture<ICommand<Result<FakeData>>, Result<FakeData>>();
        }

        #region Verify command handler result returned as expected

        [Theory]
        [MemberData(nameof(CommonHelpers.GetResultTypes), MemberType = typeof(CommonHelpers)]
        public async Task Handle_NonGenericResultQueryBehavior_Should_Execute_Handler_And_Return_It_Result(Result commandResult)
        {
            // Arrange
            var successHandler = _nonGenericResultQueryBehavior.HandlerDelegate(commandResult);

            // Act
            var result = await _nonGenericResultQueryBehavior
                .Behavior
                .Handle(new FakeQuery(), CancellationToken.None, successHandler);

            // Assert
            result.Should().Be(commandResult);
        }

        [Theory]
        [MemberData(nameof(CommonHelpers.GetGenericResultTypes), MemberType = typeof(CommonHelpers))]
        public async Task Handle_GenericResultQueryBehavior_Should_Execute_Handler_And_Return_It_Result(Result<FakeData> commandResult)
        {
            // Arrange
            var handler = _genericResultQueryBehavior.HandlerDelegate(commandResult);

            // Act
            var result = await _genericResultQueryBehavior
                .Behavior
                .Handle(new FakeGenericQuery(), CancellationToken.None, handler);

            // Assert
            result.Should().Be(commandResult);
        }

        [DynamicData(nameof(CommonHelpers.GetResultTypes), typeof(CommonHelpers), DynamicDataSourceType.Method)]
        [TestMethod]
        public async Task Handle_NonGenericResultCommandBehavior_Should_Execute_Handler_And_Return_It_Result(Result commandResult)
        {
            // Arrange
            var successHandler = _nonGenericResultCommandBehavior.HandlerDelegate(commandResult);

            // Act
            var result = await _nonGenericResultCommandBehavior
                .Behavior
                .Handle(new FakeCommand(), CancellationToken.None, successHandler);

            // Assert
            result.Should().Be(commandResult);
        }

        [DynamicData(nameof(CommonHelpers.GetGenericResultTypes), typeof(CommonHelpers), DynamicDataSourceType.Method)]
        [TestMethod]
        public async Task Handle_GenericResultCommandBehavior_Should_Execute_Handler_And_Return_It_Result(Result<FakeData> commandResult)
        {
            // Arrange
            var handler = _genericResultCommandBehavior.HandlerDelegate(commandResult);

            // Act
            var result = await _genericResultCommandBehavior
                .Behavior
                .Handle(new FakeGenericCommand(), CancellationToken.None, handler);

            // Assert
            result.Should().Be(commandResult);
        }

        #endregion

        #region Verify that behavior does not swallow exceptions throwed by command handler

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Handle_NonGenericResultQueryBehavior_CommandHandler_Throws_Should_Propagate_Exception()
        {
            // Arrange
            var successHandler = _nonGenericResultQueryBehavior.ExceptionHandlerDelegate();

            // Act
            await _nonGenericResultQueryBehavior
                .Behavior
                .Handle(new FakeQuery(), CancellationToken.None, successHandler);

            // Assert
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Handle_GenericResultQueryBehavior_CommandHandler_Throws_Should_Propagate_Exception()
        {
            // Arrange
            var successHandler = _genericResultQueryBehavior.ExceptionHandlerDelegate();

            // Act
            await _genericResultQueryBehavior
                .Behavior
                .Handle(new FakeGenericQuery(), CancellationToken.None, successHandler);

            // Assert
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Handle_NonGenericResultCommandBehavior_CommandHandler_Throws_Should_Propagate_Exception()
        {
            // Arrange
            var successHandler = _nonGenericResultCommandBehavior.ExceptionHandlerDelegate();

            // Act
            await _nonGenericResultCommandBehavior
                .Behavior
                .Handle(new FakeCommand(), CancellationToken.None, successHandler);

            // Assert
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Handle_GenericResultCommandBehavior_CommandHandler_Throws_Should_Propagate_Exception()
        {
            // Arrange
            var successHandler = _genericResultCommandBehavior.ExceptionHandlerDelegate();

            // Act
            await _genericResultCommandBehavior
                .Behavior
                .Handle(new FakeGenericCommand(), CancellationToken.None, successHandler);

            // Assert
        }

        #endregion

        private class LoggingBehaviorFixture<TRequest, TResponse>
        {
            public ILogger<LoggingBehaviour<TRequest, TResponse>> Logger { get; }

            public LoggingBehaviorSetting Setting { get; }

            public LoggingBehaviour<TRequest, TResponse> Behavior { get; }

            public LoggingBehaviorFixture()
            {
                Logger = Mock.Of<ILogger<LoggingBehaviour<TRequest, TResponse>>>();
                Setting = new LoggingBehaviorSetting(enableRequestPayloadTrace: false, enableResponsePayloadTrace: false);
                Behavior = new LoggingBehaviour<TRequest, TResponse>(Logger, Setting);
            }

            public RequestHandlerDelegate<Result> HandlerDelegate(Result result)
                => () => Task.FromResult(result);

            public RequestHandlerDelegate<Result<FakeData>> HandlerDelegate(Result<FakeData> result)
                => () => Task.FromResult(result);

            /// <summary>
            /// Used to represent command handler which throws an exception.
            /// </summary>
            public RequestHandlerDelegate<TResponse> ExceptionHandlerDelegate()
                => () => throw new InvalidOperationException();
        }
    }
}
