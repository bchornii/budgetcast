using Newtonsoft.Json;
using System;

namespace BudgetCast.Expenses.Tests.Unit.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Performs a deep copy of <paramref name="source"/> parameter object.
        /// </summary>
        /// <remarks>
        /// Private members are not cloned using this method.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T CloneJson<T>(this T source)
        {
            if (ReferenceEquals(source, null))
            {
                return default;
            }

            // initialize inner objects individually
            // for example in default constructor some list property initialized with some values,
            // but in 'source' these items are cleaned -
            // without ObjectCreationHandling.Replace default constructor values will be added to result
            var deserializeSettings = new JsonSerializerSettings
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace
            };

            return JsonConvert.DeserializeObject<T>(
                JsonConvert.SerializeObject(source),
                deserializeSettings);
        }
    }
}
