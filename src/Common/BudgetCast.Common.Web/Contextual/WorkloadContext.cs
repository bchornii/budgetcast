using Microsoft.AspNetCore.Http;

namespace BudgetCast.Common.Web.Contextual;

/// <summary>
/// Provides an abstraction over any context (either HTTP or async message handler). Allows
/// to add/retrieve any information in a scope of this context.
/// </summary>
public class WorkloadContext
{
    private readonly IDictionary<object, object> _items;

    public WorkloadContext()
    {
        _items = new Dictionary<object, object>();
    }

    /// <summary>
    /// Verifies if specified key exists.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Contains(object key)
        => _items.ContainsKey(key);
    
    /// <summary>
    /// Retrieves item by key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public object GetItem(object key)
        => _items[key];
    
    /// <summary>
    /// Adds value into collection of items via specific key.
    /// </summary>
    /// <remarks>
    /// If <see cref="HttpContext"/> is available items is also
    /// added into <see cref="HttpContext.Items"/> collection.
    /// </remarks>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    public void AddItem(object key, object value) 
        => _items[key] = value;

    public override string ToString() 
        => string.Join("-", _items.Select(x => $"{x.Key}:{x.Value}"));
}