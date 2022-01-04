using System.Text.Json;

namespace BudgetCast.Common.Application
{
    public static class AppConstants
    {
        public static readonly JsonSerializerOptions DefaultOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };
    }
}
