using BudgetCast.Dashboard.Commands.Results;
using MediatR;

namespace BudgetCast.Dashboard.Commands.Commands
{
    public class CreateMonthlyCampaignCommand : 
        IRequest<CommandResult<string>>
    {
        public string Name { get; set; }
    }
}
