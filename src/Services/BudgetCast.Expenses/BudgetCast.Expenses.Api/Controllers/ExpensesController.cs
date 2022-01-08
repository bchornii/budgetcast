using BudgetCast.Common.Web.Extensions;
using BudgetCast.Expenses.Commands.Expenses;
using BudgetCast.Expenses.Queries.Expenses.GetCampaingExpenses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BudgetCast.Expenses.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpensesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExpensesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(
            [FromQuery] GetCampaingExpensesQuery query)
        {
            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync(
            [FromBody] AddExpenseCommand command)
        {
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }
    }
}
