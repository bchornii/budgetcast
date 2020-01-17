using BudgetCast.Dashboard.Commands.Results;
using MediatR;

namespace BudgetCast.Dashboard.Commands.Commands
{
    public class AddDefaultTagCommand : 
        IRequest<CommandResult>
    {
        public string UserId { get; set; }
        public string[] Tags { get; set; }
    }
}
