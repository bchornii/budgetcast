using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BudgetCast.Dashboard.Compensations
{
    public class CompensationActionsFactory : ICompensationActionsFactory
    {
        private readonly IReadOnlyCollection<ICompensationAction> _actions;

        public CompensationActionsFactory(IReadOnlyCollection<ICompensationAction> actions)
        {
            _actions = actions;
        }
        public bool TryGet(string routeName, out ICompensationAction action)
        {
            action = _actions.FirstOrDefault(a => a.GetType()
                .GetCustomAttribute<CompensationActionAttribute>()?.Name == routeName);
            return action != null;
        }
    }
}