using System.Collections.Generic;
using System.Linq;
using BudgetCast.Common.Domain.Results;

namespace BudgetCast.Common.Application.Tests.Unit.Stubs
{
    public static class CommonHelpers
    {
        #region Non-generic result factory methods

        public static IEnumerable<object[]> GetResultTypes()
            => GetSuccessResultTypes().Concat(GetFailResultTypes());

        public static IEnumerable<object[]> GetSuccessResultTypes()
        {
            yield return new object[]
            {
                new Success(),
            };
        }

        public static IEnumerable<object[]> GetFailResultTypes()
        {
            yield return new object[]
            {
                new GeneralFail(),
            };

            yield return new object[]
            {
                new InvalidInput(),
            };

            yield return new object[]
            {
                new NotFound(),
            };
        }

        #endregion

        #region Generic result type factory methods

        public static IEnumerable<object[]> GetGenericResultTypes()
            => GetSuccessGenericResultTypes().Concat(GetFailGenericResultTypes());

        public static IEnumerable<object[]> GetSuccessGenericResultTypes()
        {
            yield return new object[]
            {
                new Success<FakeData>(new FakeData()),
            };
        }

        public static IEnumerable<object[]> GetFailGenericResultTypes()
        {
            yield return new object[]
            {
                new GeneralFail<FakeData>(),
            };

            yield return new object[]
            {
                new InvalidInput<FakeData>(),
            };

            yield return new object[]
            {
                new NotFound<FakeData>(),
            };
        }

        #endregion
    }
}
