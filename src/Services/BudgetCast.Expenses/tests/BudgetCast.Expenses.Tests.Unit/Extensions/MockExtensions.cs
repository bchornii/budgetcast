using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BudgetCast.Expenses.Tests.Unit.Extensions
{
    public static class MockExtensions
    {
        public static IEnumerable<object> GetExecutionArgumentsOf<TMock>(this Mock<TMock> mock, string methodName)
            where TMock : class
        {
            return mock
                .Invocations
                .First(i => i.Method.Name == methodName)
                .Arguments;
        }

        public static IEnumerable<object> GetExecutionArgumentsOf<TMock>(this Mock<TMock> mock, MethodInfo methodInfo)
            where TMock : class
        {
            return mock
                .Invocations
                .First(i => i.Method.AreMethodsEqualForDeclaringType(methodInfo))
                .Arguments;
        }

        public static T FirstArgumentOf<T>(this IEnumerable<object> arguments)
            where T : class
        {
            var argType = typeof(T);
            return (T)arguments.First(x => x.GetType() == argType || x.GetType().IsAssignableTo(argType));
        }

        private static bool AreMethodsEqualForDeclaringType(this MethodInfo first, MethodInfo second)
        {
            first = first.ReflectedType == first.DeclaringType
                ? first
                : first.DeclaringType!.GetMethod(first.Name, first.GetParameters().Select(p => p.ParameterType).ToArray())!;

            second = second.ReflectedType == second.DeclaringType
                ? second
                : second.DeclaringType!.GetMethod(second.Name, second.GetParameters().Select(p => p.ParameterType).ToArray())!;

            return first == second;
        }
    }
}
