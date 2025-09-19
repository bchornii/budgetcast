using BudgetCast.Common.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BudgetCast.Common.Application.Behavior.Logging
{
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
    public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
    {
        private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;
        private readonly LoggingBehaviorSetting _setting;

        public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger, LoggingBehaviorSetting setting)
        {
            _logger = logger;
            _setting = setting;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var commandName = request!.GetGenericTypeName();
            var requestType = request.GetRequestType<TRequest, TResponse>();

            try
            {
                if (_setting.EnableRequestPayloadTrace)
                {
                    var payload = JsonConvert.SerializeObject(request);
                    _logger.LogInformation("Received {CommandName} {RequestType} with payload {@Payload}", commandName, requestType, payload);
                }
                else
                {
                    _logger.LogInformation("Received {CommandName} {RequestType}", commandName, requestType);
                }

                var result = await next();

                if (_setting.EnableResponsePayloadTrace)
                {
                    var resultPayload = JsonConvert.SerializeObject(result);
                    _logger.LogInformation("{CommandName} {RequestType} processed successfully and produced {@Result} result", commandName, requestType, resultPayload);
                }
                else
                {
                    _logger.LogInformation("{CommandName} {RequestType} processed successfully", commandName, requestType);
                }

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "{CommandName} {RequestType} failed", commandName, requestType);
                throw;
            }
        }
    }
}
