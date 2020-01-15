using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BudgetCast.Dashboard.Api.Infrastructure.Extensions;
using BudgetCast.Dashboard.Api.ViewModels.Receipt;
using BudgetCast.Dashboard.Commands.Commands;
using BudgetCast.Dashboard.Commands.Results;
using BudgetCast.Dashboard.Queries.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace BudgetCast.Dashboard.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ReceiptController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        private static List<string> _catNames = new List<string>
        {            
            "Shoes & Close",
            "Car",
            "Food",
            "Healthy food",
            "Cafe & restaurants",
            "Entertaiment"            
        };

        public ReceiptController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet("categories")]
        public IActionResult GetCategories(
            [FromQuery] string name, [FromQuery] int amount)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Ok(_catNames.Take(amount).ToArray());
            }
            return Ok(_catNames
                .Where(n => n.Contains(name, System.StringComparison.OrdinalIgnoreCase))
                .OrderBy(v => v)
                .Take(amount)
                .ToArray());
        }

        [HttpPost("addBasic")]
        public async Task<IActionResult> AddBasicReceipt(
            [FromBody] AddBasicReceiptViewModel model)
        {
            var campaignResult = await _mediator
                .Send(new CampaignIdByNameQuery
                {
                    CampaignName = model.Campaign
                });

            if (campaignResult.IsSuccess())
            {
                var receiptResult = await CreateBasicReceipt(
                    model, campaignResult.Value);
                return receiptResult.ToHttpActionResult();
            }

            if (campaignResult.NotFound())
            {
                var campaignCreateResult = await _mediator
                    .Send(new CreateMonthlyCampaignCommand
                {
                    Name = model.Campaign
                });

                if (campaignCreateResult.IsSuccess())
                {
                    var receiptResult = await CreateBasicReceipt(
                        model, campaignCreateResult.Value);
                    return receiptResult.ToHttpActionResult();
                }

                return campaignCreateResult.ToHttpActionResult();
            }

            return campaignResult.ToHttpActionResult();
        }

        private async Task<CommandResult> CreateBasicReceipt(
            AddBasicReceiptViewModel model, string campaignId)
        {
            return await _mediator.Send(
                new CreateBasicReceiptCommand
                {
                    CampaignId = campaignId,
                    Tags = model.Tags,
                    Date = model.Date,
                    TotalAmount = model.TotalAmount
                });
        }
    }
}
