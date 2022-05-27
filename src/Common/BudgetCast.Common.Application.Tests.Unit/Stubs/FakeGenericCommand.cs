using BudgetCast.Common.Application.Command;
using BudgetCast.Common.Domain.Results;

namespace BudgetCast.Common.Application.Tests.Unit.Stubs
{
    public class FakeGenericCommand : ICommand<Result<FakeData>>
    {
    }
}
