using System.Threading.Tasks;
using BudgetCast.Dashboard.Api.Infrastructure.Extensions;
using BudgetCast.Dashboard.Queries.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetCast.Dashboard.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TagsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string term,
            [FromQuery] int amount)
        {
            var result = await _mediator
                .Send(new DefaultTagsQuery
                {
                    UserId = HttpContext.GetUserId(),
                    Term = term,
                    Amount = amount
                });
            return result.ToHttpActionResult();
        }
    }
}