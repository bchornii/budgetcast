using BudgetCast.Common.Application.Behavior.Authorization;
using BudgetCast.Common.Application.Command;
using BudgetCast.Common.Application.Queries;

namespace BudgetCast.Common.Application
{
    public static class RequestExtensions
    {
        private const string Command = "command";
        private const string Query = "query";
        private const string Request = "request";
        private const string AuthorizationRequirement = "authorization-requirement";

        private static readonly Type CommandType = typeof(ICommand);
        private static readonly Type QueryType = typeof(IQuery);
        private static readonly Type AuthorizationRequirementType = typeof(IAuthorizationRequirement);

        public static string GetRequestType<TRequest, TResponse>(this TRequest request)
            => true switch
            {
                _ when request.IsCommand<TRequest, TResponse>() => Command,
                _ when request.IsQuery<TRequest, TResponse>() => Query,
                _ when request.IsAuthorizationRequirement<TRequest, TResponse>() => AuthorizationRequirement,
                _ => Request,
            };

        public static bool IsCommand<TRequest, TResponse>(this TRequest command)
        {
            var t = typeof(TRequest);
            return true switch
            {
                _ when t.IsAssignableTo(CommandType) => true,
                _ when t.IsAssignableTo(typeof(ICommand<TResponse>)) => true,
                _ => false,
            };
        }

        public static bool IsQuery<TRequest, TResponse>(this TRequest query)
        {
            var t = typeof(TRequest);
            return true switch
            {
                _ when t.IsAssignableTo(QueryType) => true,
                _ when t.IsAssignableTo(typeof(IQuery<TResponse>)) => true,
                _ => false,
            };
        }

        public static bool IsAuthorizationRequirement<TRequest, TResponse>(this TRequest request)
        {
            var t = typeof(TRequest);
            return true switch
            {
                _ when t.IsAssignableTo(AuthorizationRequirementType) => true,
                _ => false,
            };
        }
    }
}
