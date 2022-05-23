using BudgetCast.Common.Application;
using BudgetCast.Common.Application.Command;
using BudgetCast.Common.Domain;
using BudgetCast.Common.Domain.Results;
using BudgetCast.Expenses.Domain.Campaigns;

namespace BudgetCast.Expenses.Commands.Campaigns
{
    public record CreateMonthlyCampaignCommand(string Name) : ICommand<Result<long>>;

    public class CreateMonthlyCampaignCommandHandler :
        ICommandHandler<CreateMonthlyCampaignCommand, Result<long>>
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateMonthlyCampaignCommandHandler(
            ICampaignRepository campaignRepository,
            IUnitOfWork unitOfWork)
        {
            _campaignRepository = campaignRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<long>> Handle(
            CreateMonthlyCampaignCommand request, 
            CancellationToken cancellationToken)
        {
            var campaign = new Campaign(request.Name);
            await _campaignRepository.AddAsync(campaign, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);

            return campaign.Id;
        }
    }
}
