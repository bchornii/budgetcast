namespace BudgetCast.Common.Domain.Results;

#region Abstract Result types

public abstract record Result
{
    private const int ValuesArrSize = 3;
    private readonly Dictionary<string, int> _keyValueIdxMap = new();

    public bool IsFailure => Errors.Count > 0 || !IsOfSuccessType();
    
    public IDictionary<string, List<string>> Errors { get; init; } = new Dictionary<string, List<string>>();
    
    public Result AddError(Error error)
    {
        if (!IsFailure)
        {
            throw new AddingErrorsToSuccessResultException(
                "Adding exceptions to success result is not allowed.");
        }

        if (Errors.ContainsKey(error.Code))
        {
            Errors[error.Code].Add(error.Value);
        }
        else
        {
            Errors.Add(error.Code, new List<string> {error.Value});
        }

        return this;
    }

    public Result AddError(IDictionary<string, List<string>> errors)
    {
        if (!IsFailure)
        {
            throw new AddingErrorsToSuccessResultException(
                "Adding exceptions to success result is not allowed.");
        }
        
        foreach (var (key, values) in errors)
        {
            if (Errors.ContainsKey(key))
            {
                Errors[key].AddRange(values);
            }
            else
            {
                Errors.Add(key, values);
            }
        }

        return this;
    }

    private bool IsOfSuccessType()
    {
        var type = GetType();
        return type.IsAssignableTo(typeof(Success)) ||
               (type.IsGenericType && type.GetGenericTypeDefinition().IsAssignableTo(typeof(Success<>)));
    }
    
    public static implicit operator Result(Error error) => GeneralFail(error);

    public static implicit operator bool(Result result) => !result.IsFailure;

    #region Creation methods - success

    public static Success Success() => new();

    public static Success<T> Success<T>(T value) 
        where T : notnull => new(value);
    
    #endregion
    
    #region Creation methods - general fail

    public static GeneralFail GeneralFail() => new();
    
    public static GeneralFail GeneralFail(Error error)
    {
        var fail = new GeneralFail();
        fail.AddError(error);
        return fail;
    }
    
    public static GeneralFail GeneralFail(IDictionary<string, List<string>> errors)
    {
        var fail = new GeneralFail();
        fail.AddError(errors);
        return fail;
    }
    
    public static GeneralFail GeneralFail(Result r1)
        => GeneralFail(r1, Results.Success.Empty, Results.Success.Empty, Results.Success.Empty);
    
    public static GeneralFail GeneralFail(Result r1, Result r2)
        => GeneralFail(r1, r2, Results.Success.Empty, Results.Success.Empty);

    public static GeneralFail GeneralFail(Result r1, Result r2, Result r3)
        => GeneralFail(r1, r2, r3, Results.Success.Empty);

    public static GeneralFail GeneralFail(Result r1, Result r2, Result r3, Result r4)
    {
        var fail = GeneralFail();
        
        if (!r1)
        {
            fail.AddError(r1.Errors);
        }

        if (!r2)
        {
            fail.AddError(r2.Errors);    
        }

        if (!r3)
        {
            fail.AddError(r3.Errors);    
        }

        if (!r4)
        {
            fail.AddError(r4.Errors);
        }
        
        return fail;
    }
    
    public static GeneralFail<T> GeneralFail<T>() => new();
    
    public static GeneralFail<T> GeneralFail<T>(Error error)
    {
        var fail = new GeneralFail<T>();
        fail.AddError(error);
        return fail;
    }
    
    public static GeneralFail<T> GeneralFail<T>(IDictionary<string, List<string>> errors)
    {
        var fail = new GeneralFail<T>();
        fail.AddError(errors);
        return fail;
    }

    public static GeneralFail<T> GeneralFail<T>(Result r1)
        => GeneralFail<T>(r1, Results.Success.Empty, Results.Success.Empty, Results.Success.Empty);
    
    public static GeneralFail<T> GeneralFail<T>(Result r1, Result r2)
        => GeneralFail<T>(r1, r2, Results.Success.Empty, Results.Success.Empty);

    public static GeneralFail<T> GeneralFail<T>(Result r1, Result r2, Result r3)
        => GeneralFail<T>(r1, r2, r3, Results.Success.Empty);

    public static GeneralFail<T> GeneralFail<T>(Result r1, Result r2, Result r3, Result r4)
    {
        var fail = GeneralFail<T>();
        
        if (!r1)
        {
            fail.AddError(r1.Errors);
        }

        if (!r2)
        {
            fail.AddError(r2.Errors);    
        }

        if (!r3)
        {
            fail.AddError(r3.Errors);    
        }

        if (!r4)
        {
            fail.AddError(r4.Errors);
        }
        
        return fail;
    }
    
    #endregion
    
    #region Creation methods - invalid input
    public static InvalidInput InvalidInput() => new();
    public static InvalidInput InvalidInput(Error error)
    {
        var fail = new InvalidInput();
        fail.AddError(error);
        return fail;
    }

    public static InvalidInput<T> InvalidInput<T>() => new();
    public static InvalidInput<T> InvalidInput<T>(Error error)
    {
        var fail = new InvalidInput<T>();
        fail.AddError(error);
        return fail;
    }

    #endregion

    #region Creation methods - not found

    public static NotFound NotFound() => new();
    public static NotFound NotFound(Error error)
    {
        var fail = new NotFound();
        fail.AddError(error);
        return fail;
    }
    
    public static NotFound<T> NotFound<T>() => new();
    public static NotFound<T> NotFound<T>(Error error)
    {
        var fail = new NotFound<T>();
        fail.AddError(error);
        return fail;
    }

    #endregion
}

public abstract record Result<T> : Result
    where T : notnull
{
    private readonly T _value = default!;
    
    public T Value
    {
        get
        {
            if (_value is null)
            {
                throw new ResultValueIsNullException();
            }
            
            return _value;
        }
        protected init
        {
            if (value is null)
            {
                throw new ResultValueIsNullException();
            }
            _value = value;
        }
    }

    public static implicit operator Result<T>(Error error) => GeneralFail<T>(error);
    public static implicit operator Result<T>(T value) => Success(value);
}

#endregion

#region Success Result types

public record Success : Result
{
    public static Success Empty { get; } = new();
}

public record Success<T> : Result<T> 
    where T : notnull
{
    public Success(T value)
    {
        Value = value;
    }
    
    public static implicit operator Success<T>(T value) => new(value);
}

#endregion

#region General failure error types

public record GeneralFail : Result
{
    public static implicit operator GeneralFail(Error error) => GeneralFail(error);
}

public record GeneralFail<T> : Result<T>
{
    public static implicit operator GeneralFail<T>(Error error) => GeneralFail<T>(error);
}

#endregion

#region Invalid input error types

public record InvalidInput : GeneralFail
{
    public static implicit operator InvalidInput(Error error) => InvalidInput(error);
}

public record InvalidInput<T> : GeneralFail<T>
{
    public static implicit operator InvalidInput<T>(Error error) => InvalidInput<T>(error);
}

#endregion

#region Not-found error types

public record NotFound : GeneralFail
{
    public static implicit operator NotFound(Error error) => Result.NotFound(error);
}

public record NotFound<T> : GeneralFail<T>
{
    public static implicit operator NotFound<T>(Error error) => Result.NotFound<T>(error);
}

#endregion