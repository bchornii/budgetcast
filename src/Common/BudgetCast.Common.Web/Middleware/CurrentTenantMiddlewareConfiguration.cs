namespace BudgetCast.Common.Web.Middleware
{
    public class CurrentTenantMiddlewareConfiguration
    {
        private readonly List<string> _pathsToExclude = new()
        {
            "/swagger",
            "/jobs",
            "/hc"
        };

        public IReadOnlyList<string> PathsToExlude => _pathsToExclude;

        public void AddRange(string[] paths)
            => _pathsToExclude.AddRange(paths);

        public void Add(string path)
            => _pathsToExclude.Add(path);
    }
}
