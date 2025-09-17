using BudgetCast.Common.Application.Command;
using BudgetCast.Common.Data;
using BudgetCast.Common.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetCast.Expenses.Api.Application.Behavior;

public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> _logger;
    private readonly OperationalDbContext _dbContext;

    public TransactionBehaviour(
        OperationalDbContext dbContext, 
        ILogger<TransactionBehaviour<TRequest, TResponse>> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_dbContext.HasActiveTransaction)
        {
            return await next();
        }
        
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(_dbContext, async (dbCxt, token) =>
        {
            var typeName = request.GetGenericTypeName();
            await using var transaction = await dbCxt.BeginTransactionAsync(token);
            
            _logger.LogInformation("----- Begin transaction {TransactionId} for {CommandName}", transaction.TransactionId, typeName);
            var response = await next();
            _logger.LogInformation("----- Commit transaction {TransactionId} for {CommandName}", transaction.TransactionId, typeName);

            await dbCxt.CommitTransactionAsync(transaction, token);
            
            return response;
        }, cancellationToken);
    }
}