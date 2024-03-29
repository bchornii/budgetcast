﻿using BudgetCast.Common.Web.Extensions;
using BudgetCast.Expenses.Commands.Campaigns;
using BudgetCast.Expenses.Commands.Campaigns.CreateMonthlyCampaign;
using BudgetCast.Expenses.Queries.Campaigns.GetCampaignByName;
using BudgetCast.Expenses.Queries.Campaigns.GetCampaigns;
using BudgetCast.Expenses.Queries.Campaigns.GetCampaignTotals;
using BudgetCast.Expenses.Queries.Campaigns.SearchForExistingCampaignsByName;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetCast.Expenses.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CampaignsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var result = await _mediator.Send(new GetCampaignsQuery());
            return result.ToActionResult();
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetByNameAsync(
            [FromRoute] string name)
        {
            var result = await _mediator
                .Send(new GetCampaignByNameQuery(name));
            return result.ToActionResult();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchForNamesAsync(
            [FromQuery] string term,
            [FromQuery] int amount = 10)
        {
            var result = await _mediator.Send(
                new SearchForExistingCampaignsByNameQuery(amount, term));
            return result.ToActionResult();
        }

        [HttpGet("{name}/totals")]
        public async Task<IActionResult> GetCampaignTotalsAsync(
            [FromRoute] string name)
        {
            var result = await _mediator.Send(new GetCampaignTotalsQuery(name));
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
