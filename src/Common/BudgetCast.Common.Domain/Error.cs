namespace BudgetCast.Common.Domain;

public sealed class Error : ValueObject
{
    private const char Separator = '=';
    
    /// <summary>
    /// Represents error code which is part of the contract with the clients.
    /// As an example, it may contain property name which triggered the error,
    /// or any other code-like identifier.
    /// </summary>
    public string Code { get; }
        
    /// <summary>
    /// Represents error value and in general is for debugging purposes only. As an example, it may
    /// contain message text which describes the error code. 
    /// </summary>
    public string Value { get; }

    public Error(string code, string message)
    {
        Code = code;
        Value = message;
    }
        
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
    }
}