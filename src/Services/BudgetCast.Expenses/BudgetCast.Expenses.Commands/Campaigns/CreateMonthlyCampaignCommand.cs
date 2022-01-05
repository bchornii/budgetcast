using BudgetCast.Common.Application;
using BudgetCast.Common.Application.Command;
using BudgetCast.Common.Domain;
using BudgetCast.Expenses.Domain.Campaigns;

namespace BudgetCast.Expenses.Commands.Campaigns
{
    public record CreateMonthlyCampaignCommand : ICommand<Result<ulong>>
    {
        public string Name { get; set; }

        public CreateMonthlyCampaignCommand()
        {
            Name = default!;
        }
    }

    public class CreateMonthlyCampaignCommandHandler :
        ICommandHandler<CreateMonthlyCampaignCommand, Result<ulong>>
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

        public async Task<Result<ulong>> Handle(
            CreateMonthlyCampaignCommand request, 
            CancellationToken cancellationToken)
        {
            var campaign = new Campaign(request.Name);
            await _campaignRepository.Add(campaign);
            await _unitOfWork.Commit();
            return new Success<ulong>(campaign.Id);
        }
    }
}
