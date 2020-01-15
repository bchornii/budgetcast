using System.Collections.Generic;

namespace BudgetCast.Dashboard.Commands.Results
{
    public enum ResultStatus
    {
        Success,
        ValidationViolation,
        Failed
    }

    public class CommandResult<T> : CommandResult
    {
        public T Value { get; set; }
    }

    public class CommandResult
    {
        public CommandResult()
        {
            Errors = new List<string>();
        }

        public ResultStatus Status { get; private set; }

        public bool IsSuccess() => Status == ResultStatus.Success;

        public List<string> Errors { get; }

        public static CommandResult GetValidationViolationResult(string msg) =>
            new CommandResult
            {
                Status = ResultStatus.ValidationViolation,
                Errors = { msg }
            };

        public static CommandResult GetSuccessResult() =>
            new CommandResult
            {
                Status = ResultStatus.Success
            };

        public static CommandResult<T> GetSuccessResult<T>(T value) =>
            new CommandResult<T>
            {
                Status = ResultStatus.Success,
                Value = value
            };

        public static CommandResult GetFailedResult(string msg) =>
            new CommandResult
            {
                Status = ResultStatus.Failed,
                Errors = { msg }
            };

        public static CommandResult<T> GetFailedResult<T>(string msg) =>
            new CommandResult<T>
            {
                Status = ResultStatus.Failed,
                Errors = { msg }
            };
    }
}
