namespace BudgetCast.Common.Extensions
{
    public static class GenericTypeExtensions
    {
        public static string GetGenericTypeName(this Type type)
        {
            if (!type.IsGenericType)
            {
                return type.Name;
            }

            var genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name).ToArray());
            return $"{type.Name.Remove(startIndex: type.Name.IndexOf('`', StringComparison.OrdinalIgnoreCase))}<{genericTypes}>";
        }

        public static string GetGenericTypeName(this object value)
        {
            return value.GetType().GetGenericTypeName();
        }

        public static object CreateInstanceOf(this Type type, Type genericTypeArgument)
        {
            var resultType = type.MakeGenericType(genericTypeArgument);
            return Activator.CreateInstance(resultType)!;
        }
    }
}
