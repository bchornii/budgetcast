using System.Threading;
using System.Threading.Tasks;
using BudgetCast.Dashboard.Commands.Commands;
using BudgetCast.Dashboard.Commands.Results;
using BudgetCast.Dashboard.Domain.Aggregates.Campaigns;
using MediatR;

namespace BudgetCast.Dashboard.Commands.Handlers
{
    public class CreateMonthlyCampaignCommandHandler :
        IRequestHandler<CreateMonthlyCampaignCommand, CommandResult<string>>
    {
        private readonly ICampaignRepository _campaignRepository;

        public CreateMonthlyCampaignCommandHandler(ICampaignRepository campaignRepository)
        {
            _campaignRepository = campaignRepository;
        }

        public async Task<CommandResult<string>> Handle(CreateMonthlyCampaignCommand request, 
            CancellationToken cancellationToken)
        {
            var campaign = await _campaignRepository.Add(new Campaign(request.Name));
            return CommandResult.GetSuccessResult(campaign.Id);
        }
    }
}
