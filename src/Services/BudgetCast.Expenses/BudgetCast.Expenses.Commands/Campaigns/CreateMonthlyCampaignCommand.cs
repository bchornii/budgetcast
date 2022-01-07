using BudgetCast.Common.Application;
using BudgetCast.Common.Application.Command;
using BudgetCast.Common.Domain;
using BudgetCast.Expenses.Domain.Campaigns;

namespace BudgetCast.Expenses.Commands.Campaigns
{
    public record CreateMonthlyCampaignCommand : ICommand<Result<long>>
    {
        public string Name { get; set; }

        public CreateMonthlyCampaignCommand()
        {
            Name = default!;
        }
    }

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
            await _campaignRepository.Add(campaign);
            await _unitOfWork.Commit();
            return new Success<long>(campaign.Id);
        }
    }
}
