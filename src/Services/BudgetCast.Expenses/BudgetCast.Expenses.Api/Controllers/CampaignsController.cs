using BudgetCast.Common.Web.Extensions;
using BudgetCast.Expenses.Commands.Campaigns;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BudgetCast.Expenses.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CampaignsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("monthly")]
        public async Task<IActionResult> CreateMonthly(
            [FromBody] CreateMonthlyCampaignCommand command)
        {
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }
    }
}
