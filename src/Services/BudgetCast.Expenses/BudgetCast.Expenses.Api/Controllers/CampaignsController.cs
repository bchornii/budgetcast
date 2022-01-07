﻿using BudgetCast.Common.Web.Extensions;
using BudgetCast.Expenses.Commands.Campaigns;
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

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByNameAsync(
            [FromRoute] string name)
        {
            var result = await _mediator
                .Send(new GetCampaignByNameQuery
                {
                    Name = name
                });
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