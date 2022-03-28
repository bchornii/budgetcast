using BudgetCast.Common.Operations;
using BudgetCast.Common.Web.Contextual;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace BudgetCast.Common.Web.Filters;

/// <summary>
/// Retrieves operation context and add [controller-name]_[action-name] as a part
/// </summary>
public class OperationFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var operationContext = context.HttpContext
            .RequestServices.GetRequiredService<OperationContext>();

        var controllerName = ((ControllerBase)context.Controller)
            .ControllerContext.ActionDescriptor.ControllerName;
        var actionName = ((ControllerBase)context.Controller)
            .ControllerContext.ActionDescriptor.ActionName;

        operationContext.Add(new OperationPart($"{controllerName}_{actionName}"));
        
        // Save operation context into HttpContext.Items in order to use it afterwards
        // in OperationHeaderHandler since handlers are resolved from other DI scope than
        // request. See more details in the topic below:
        // https://andrewlock.net/understanding-scopes-with-ihttpclientfactory-message-handlers/#accessing-the-request-scope-from-a-custom-httpmessagehandler
        context.HttpContext.Items[OperationContext.MetaName] = operationContext;

        base.OnActionExecuting(context);
    }
}