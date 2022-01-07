﻿using System.Reflection;

namespace BudgetCast.Common.Application
{
    public static class ResultExtensions
    {
        private static readonly Type _successType = typeof(Success);

        public static bool IsGenericResult(this Type type)
            => type.IsGenericType && type.IsAssignableTo(typeof(Result));

        public static Type GetGenericResultArgumentType(this Type type)
            => type.GetGenericArguments().First();

        public static Result WithErrors(this object result, IDictionary<string, string[]> errors)
        {
            var resultType = result.GetType();
            var errorsProperty = resultType.GetProperty(
                nameof(NotFound.Errors),
                BindingFlags.Public | BindingFlags.Instance);
            errorsProperty?.SetValue(result, errors, null);
            return (Result)result;
        }

        public static bool IsSuccess(this Result result) 
            => result is Success;

        public static bool IsSuccess<T>(this Result<T> result)
            => result is Success<T>;

        public static bool IsSuccess<T>(this Result<T> result, out Success<T> successResult)
        {
            if(result is Success<T> success)
            {
                successResult = success;
                return true;
            }
            successResult = default!;
            return false;
        }

        public static (bool IsOfSuccessType, bool IsGeneric) CheckIfSuccess(this object response)
        {
            var responseType = response.GetType();

            if (responseType == _successType)
            {
                return (IsOfSuccessType: true, IsGeneric: false);
            }

            if (responseType.IsGenericType)
            {
                var genericArgumentType = responseType
                    .GetGenericResultArgumentType();
                var genericSuccessType = typeof(Success<>)
                    .MakeGenericType(genericArgumentType);

                if (responseType == genericSuccessType)
                {
                    return (IsOfSuccessType: true, IsGeneric: true);
                }
            }

            return (IsOfSuccessType: false, IsGeneric: false);
        }
    }
}