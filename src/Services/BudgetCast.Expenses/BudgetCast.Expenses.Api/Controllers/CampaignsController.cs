using BudgetCast.Common.Web.Extensions;
using BudgetCast.Expenses.Commands.Campaigns;
using BudgetCast.Expenses.Queries.Campaigns.FindForExistingCampaignsByName;
using BudgetCast.Expenses.Queries.Campaigns.GetCampaignByName;
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

        [HttpGet("name")]
        public async Task<IActionResult> GetByNameAsync(
            [FromQuery] string value)
        {
            var result = await _mediator
                .Send(new GetCampaignByNameQuery
                {
                    Name = value
                });
            return result.ToActionResult();
        }

        [HttpGet("searchForNames")]
        public async Task<IActionResult> SearchForNamesAsync(
            [FromQuery] string campaignNameTerm,
            [FromQuery] int amount = 10)
        {
            var result = await _mediator.Send(
                new FindForExistingCampaignsByNameQuery(amount, campaignNameTerm));
            return result.ToActionResult();
        }

        [HttpPost("monthly")]
        public async Task<IActionResult> CreateMonthlyAsync(
            [FromBody] CreateMonthlyCampaignCommand command)
        {
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }
    }
}
