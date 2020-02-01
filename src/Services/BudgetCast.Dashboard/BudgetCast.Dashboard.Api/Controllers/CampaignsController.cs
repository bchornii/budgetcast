using System.Threading.Tasks;
using BudgetCast.Dashboard.Api.Infrastructure.Extensions;
using BudgetCast.Dashboard.Queries.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BudgetCast.Dashboard.Api.Controllers
{
    [Route("api/[controller]")]
    //[Authorize]
    [ApiController]
    public class CampaignsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CampaignsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetCampaigns()
        {
            var result = await _mediator
                .Send(new UserCampaignsQuery
                {
                    UserId = HttpContext.GetUserId()
                });
            return result.ToHttpActionResult();
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetCampaigns(
            [FromQuery] string term,
            [FromQuery] int amount)
        {
            var result = await _mediator
                .Send(new DefaultCampaignsQuery
                {
                    Term = term,
                    Amount = amount
                });
            return result.ToHttpActionResult();
        }
    }
}