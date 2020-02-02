using BudgetCast.Dashboard.Domain.ReadModel.Receipts;
using BudgetCast.Dashboard.Queries.Results;
using MediatR;

namespace BudgetCast.Dashboard.Queries.Queries
{
    public class ReceiptDetailsQuery : IRequest<
        QueryResult<Receipt>>
    {
        public string ReceiptId { get; set; }
    }
}