using BudgetCast.Common.Application;
using BudgetCast.Common.Application.Queries;

namespace BudgetCast.Expenses.Queries.Expenses.GetCampaingExpenses
{
    public record GetCampaingExpensesQuery : 
        IQuery<Result<ExpenseViewModel>>
    {
        public string CampaignName { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public GetCampaingExpensesQuery()
        {
            CampaignName = default!;
        }
    }

    public class GetCampaingExpensesQueryHandler : 
        IQueryHandler<GetCampaingExpensesQuery, Result<ExpenseViewModel>>
    {
        public Task<Result<ExpenseViewModel>> Handle(
            GetCampaingExpensesQuery request, 
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
