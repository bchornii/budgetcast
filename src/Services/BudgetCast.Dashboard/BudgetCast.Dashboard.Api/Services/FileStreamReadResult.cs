namespace BudgetCast.Dashboard.Api.Services
{
    public class FileStreamReadResult
    {
        public bool Success { get; }
        public string Error { get; }
        public byte[] Content { get; }
        public string FileName { get; }
        public long Size => Content?.Length ?? 0;

        public FileStreamReadResult(bool success, 
            string error, string fileName, byte[] content)
        {
            Success = success;
            Error = error;
            FileName = fileName;
            Content = content;
        }
    }
}