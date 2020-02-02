using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
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

        public ReceiptController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetReceiptItems(string id)
        {
            var result = await _mediator.Send(new ReceiptDetailsQuery
            {
                ReceiptId = id
            });
            return result.ToHttpActionResult();
        }

        [HttpGet("basicReceipts")]
        public async Task<IActionResult> GetBasicReceipts(
            [FromQuery] string campaignName,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new BasicReceiptsQuery
            {
                CampaignName = campaignName,
                Page = page,
                PageSize = pageSize,
                UserId = HttpContext.GetUserId()
            });
            return result.ToHttpActionResult();
        }

        [HttpGet("total/{campaignName}")]
        public async Task<IActionResult> GetTotalSpentAmount(string campaignName)
        {
            var result = await _mediator.Send(
                new TotalSpentAmountPerCampaignQuery
                {
                    CampaignName = campaignName,
                    UserId = HttpContext.GetUserId()
                });
            return result.ToHttpActionResult();
        }

        [HttpPost("addBasic")]
        public async Task<IActionResult> AddBasicReceipt(
            [FromBody] AddBasicReceiptViewModel model)
        {
            await _mediator.Send(
                new AddDefaultTagCommand
            {
                Tags = model.Tags,
                UserId = HttpContext.GetUserId()
            });

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
                    TotalAmount = model.TotalAmount,
                    Description = model.Description
                });
        }
    }
}
