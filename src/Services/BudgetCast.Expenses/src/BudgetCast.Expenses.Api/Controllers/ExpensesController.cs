using BudgetCast.Common.Web.Extensions;
using BudgetCast.Expenses.Commands.Expenses;
using BudgetCast.Expenses.Queries.Expenses.GetExpenseById;
using BudgetCast.Expenses.Queries.Expenses.GetExpensesForCampaign;
using BudgetCast.Expenses.Queries.Expenses.SearchForExistingTagsByName;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetCast.Expenses.Api.Controllers
{
    [Authorize]
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
            [FromQuery] GetExpensesForCampaignQuery query)
        {
            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(
            [FromRoute] long id)
        {
            var result = await _mediator.Send(new GetExpenseByIdQuery(id));
            return result.ToActionResult();
        }

        [HttpGet("tags/search")]
        public async Task<IActionResult> SearchForTagsAsync(
            [FromQuery] string term,
            [FromQuery] int amount = 10)
        {
            var result = await _mediator.Send(new SearchForExistingTagsByNameQuery(amount, term));
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
