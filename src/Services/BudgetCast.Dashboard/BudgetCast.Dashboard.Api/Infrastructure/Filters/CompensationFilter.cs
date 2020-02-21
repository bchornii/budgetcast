using System.Linq;
using System.Threading.Tasks;
using BudgetCast.Dashboard.Compensations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;

namespace BudgetCast.Dashboard.Api.Infrastructure.Filters
{
    public class CompensationFilter : ExceptionFilterAttribute
    {
        private readonly ICompensationActionsFactory _compensationActionsFactory;

        public CompensationFilter(ICompensationActionsFactory compensationActionsFactory)
        {
            _compensationActionsFactory = compensationActionsFactory;
        }

        public override async Task OnExceptionAsync(ExceptionContext context)
        {
            var routeName = (context.ActionDescriptor?.EndpointMetadata.FirstOrDefault(m =>
                    m.GetType().BaseType == typeof(HttpMethodAttribute)) as HttpMethodAttribute)?.Name;

            if (!string.IsNullOrWhiteSpace(routeName) &&
                _compensationActionsFactory.TryGet(routeName, out var action))
            {
                await action.Compensate();
            }
        }
    }
}
