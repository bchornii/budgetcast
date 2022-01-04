namespace BudgetCast.Common.Application.Behavior.Logging
{
    public class LoggingBehaviorSetting
    {
        public LoggingBehaviorSetting(bool enableRequestPayloadTrace, bool enableResponsePayloadTrace)
        {
            EnableRequestPayloadTrace = enableRequestPayloadTrace;
            EnableResponsePayloadTrace = enableResponsePayloadTrace;
        }

        public bool EnableRequestPayloadTrace { get; }

        public bool EnableResponsePayloadTrace { get; }
    }
}
