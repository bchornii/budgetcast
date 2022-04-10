namespace BudgetCast.Common.Operations;

public sealed class OperationPart
{
    public OperationPart(string name)
    {
        Name = name;
    }

    public string Name { get; }
}