using System;
using System.Linq;
using System.Reflection;

namespace BudgetCast.Dashboard.Data
{
    public static class MongoMapsRegistrator
    {
        public static void RegisterDocumentMaps()
        {
            var assembly = Assembly.GetAssembly(typeof(MongoMapsRegistrator));

            var classMaps = assembly.GetTypes()
                .Where(t => t.BaseType != null && t.BaseType.IsGenericType &&
                            t.BaseType.GetGenericTypeDefinition() == typeof(MongoDbClassMap<>));

            foreach (var classMap in classMaps)
            {
                Activator.CreateInstance(classMap);
            }
        }
    }
}
