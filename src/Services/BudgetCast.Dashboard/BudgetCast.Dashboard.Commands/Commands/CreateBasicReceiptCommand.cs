using System;
using BudgetCast.Dashboard.Commands.Results;
using MediatR;

namespace BudgetCast.Dashboard.Commands.Commands
{
    public class CreateBasicReceiptCommand : IRequest<CommandResult>
    {
        public DateTime Date { get; set; }
        public string[] Tags { get; set; }
        public string CampaignId { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
