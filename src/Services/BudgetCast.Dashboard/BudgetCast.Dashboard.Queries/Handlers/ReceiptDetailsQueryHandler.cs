using System.Threading;
using System.Threading.Tasks;
using BudgetCast.Dashboard.Domain.ReadModel.Receipts;
using BudgetCast.Dashboard.Queries.Queries;
using BudgetCast.Dashboard.Queries.Results;
using MediatR;

namespace BudgetCast.Dashboard.Queries.Handlers
{
    public class ReceiptDetailsQueryHandler : IRequestHandler<
        ReceiptDetailsQuery, QueryResult<Receipt>>
    {
        private readonly IReceiptReadAccessor _receiptReadAccessor;

        public ReceiptDetailsQueryHandler(IReceiptReadAccessor receiptReadAccessor)
        {
            _receiptReadAccessor = receiptReadAccessor;
        }
        public async Task<QueryResult<Receipt>> Handle(
            ReceiptDetailsQuery request, CancellationToken cancellationToken)
        {
            var result = await _receiptReadAccessor.GetReceiptDetails(request.ReceiptId);
            return QueryResult<Receipt>.GetSuccessResult(result);
        }
    }
}
