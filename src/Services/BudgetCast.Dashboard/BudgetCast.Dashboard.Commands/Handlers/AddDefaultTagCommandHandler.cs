using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BudgetCast.Dashboard.Commands.Commands;
using BudgetCast.Dashboard.Commands.Results;
using BudgetCast.Dashboard.Domain.AnemicModel;
using BudgetCast.Dashboard.Domain.Extensions;
using BudgetCast.Dashboard.Domain.ReadModel.Tags;
using MediatR;
using static BudgetCast.Dashboard.Commands.Results.CommandResult;

namespace BudgetCast.Dashboard.Commands.Handlers
{
    public class AddDefaultTagCommandHandler : IRequestHandler<
        AddDefaultTagCommand, CommandResult>
    {
        private readonly IDefaultTagWriteAccessor _tagWriteAccessor;
        private readonly IDefaultTagReadAccessor _tagReadAccessor;

        public AddDefaultTagCommandHandler(
            IDefaultTagWriteAccessor tagWriteAccessor,
            IDefaultTagReadAccessor tagReadAccessor)
        {
            _tagWriteAccessor = tagWriteAccessor;
            _tagReadAccessor = tagReadAccessor;
        }
        public async Task<CommandResult> Handle(AddDefaultTagCommand request, 
            CancellationToken cancellationToken)
        {
            if (request.Tags.IsEmpty())
            {
                return GetSuccessResult();
            }

            var existingTags = await _tagReadAccessor
                .GetExistingTags(request.Tags);

            var nonExistingTags = request.Tags
                .Where(t => !existingTags.Contains(t))
                .Select(t => new DefaultTag
                {
                    Name = t, 
                    UserId = request.UserId
                })
                .ToArray();

            if (nonExistingTags.Any())
            {
                await _tagWriteAccessor.AddTags(nonExistingTags);
            }

            return GetSuccessResult();
        }
    }
}
