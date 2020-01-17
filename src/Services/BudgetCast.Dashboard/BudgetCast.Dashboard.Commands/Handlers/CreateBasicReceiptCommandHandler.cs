using System.Threading;
using System.Threading.Tasks;
using BudgetCast.Dashboard.Commands.Commands;
using BudgetCast.Dashboard.Commands.Results;
using BudgetCast.Dashboard.Domain.Aggregates.Receipting;
using MediatR;

namespace BudgetCast.Dashboard.Commands.Handlers
{
    public class CreateBasicReceiptCommandHandler :
        IRequestHandler<CreateBasicReceiptCommand, CommandResult>
    {
        private readonly IReceiptRepository _receiptRepository;

        public CreateBasicReceiptCommandHandler(IReceiptRepository receiptRepository)
        {
            _receiptRepository = receiptRepository;
        }

        public async Task<CommandResult> Handle(CreateBasicReceiptCommand request,
            CancellationToken cancellationToken)
        {
            var receipt = new Receipt(request.Date, request.CampaignId, request.Description);
            receipt.AddTags(request.Tags);
            receipt.AddItem(Receipt.DefaultItemTitle, request.TotalAmount);
            await _receiptRepository.Add(receipt);

            return CommandResult.GetSuccessResult();
        }
    }
}
