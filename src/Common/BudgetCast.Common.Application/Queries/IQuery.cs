using MediatR;

namespace BudgetCast.Common.Application.Queries
{
    public interface IQuery
    {
    }

    public interface IQuery<out TResult> : IQuery, IRequest<TResult>
    {
    }
}
