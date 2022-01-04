using MediatR;

namespace BudgetCast.Common.Application.Command
{
    public interface ICommand : IRequest
    {
    }
    public interface ICommand<out TResult> : IRequest<TResult>
    {
    }
}
