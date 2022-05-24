namespace BudgetCast.Common.Domain.Results;

#region Abstract untyped Result

/// <summary>
/// Represents untyped operation result.
/// </summary>
public abstract record Result
{
    #region Public contract properties

    /// <summary>
    /// Verifies if underlying type of <see cref="Result"/> if of any failure type.
    /// </summary>
    public bool IsOfFailure => Errors.Count > 0 || !IsOfSuccessType();
    
    /// <summary>
    /// Represents errors hash table associated with an instance of <see cref="Result"/> type.
    /// </summary>
    public IDictionary<string, List<string>> Errors { get; init; } = new Dictionary<string, List<string>>();

    #endregion

    #region Public contract methods

    /// <summary>
    /// Adds an error to <see cref="Result"/> instance's errors hash table with
    /// value of <paramref name="error"/> passed parameter under 'general' key.
    /// </summary>
    /// <param name="error">Error text</param>
    /// <returns></returns>
    /// <exception cref="AddingErrorsToSuccessResultException"></exception>
    public Result AddErrors(string error)
    {
        if (!IsOfFailure)
        {
            throw new AddingErrorsToSuccessResultException(
                "Adding exceptions to success result is not allowed.");
        }

        const string generalKey = "general";
        
        if (Errors.ContainsKey(generalKey))
        {
            Errors[generalKey].Add(error);
        }
        else
        {
            Errors.Add(generalKey, new List<string> { error });
        }

        return this;
    }
    
    /// <summary>
    /// Adds an error to <see cref="Result"/> instance's errors hash table with
    /// key <see cref="Error.Code"/> and value of <see cref="Error.Value"/>.
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    /// <exception cref="AddingErrorsToSuccessResultException"></exception>
    public Result AddErrors(Error error)
    {
        if (!IsOfFailure)
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

    /// <summary>
    /// Merges errors hash table passed as a <paramref name="errors"/> parameter
    /// into <see cref="Result"/> instance's errors hash table.
    /// </summary>
    /// <param name="errors"></param>
    /// <returns></returns>
    /// <exception cref="AddingErrorsToSuccessResultException"></exception>
    public Result AddErrors(IDictionary<string, List<string>> errors)
    {
        if (!IsOfFailure)
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

    #endregion

    #region Implicit conversion

    /// <summary>
    /// Converts instances of <see cref="Error"/> type into <see cref="Result"/>
    /// with underlying <see cref="BudgetCast.Common.Domain.Results.GeneralFail"/> type.
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public static implicit operator Result(Error error) => GeneralFail(error);
    
    /// <summary>
    /// Converts <see cref="Result"/> into <see cref="bool"/>. Returns <c>true</c>
    /// if <see cref="IsOfFailure"/> is set to false. Return <c>false</c> when <see cref="IsOfFailure"/> is set to true.
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static implicit operator bool(Result result) => !result.IsOfFailure;

    #endregion

    #region Creation methods - success

    public static Success Success() => new();

    public static Success<T> Success<T>(T value) 
        where T : notnull => new(value);
    
    #endregion
    
    #region Creation methods - general fail - untyped

    public static GeneralFail GeneralFail() => new();
    
    public static GeneralFail GeneralFail(Error error)
    {
        var fail = GeneralFail();
        fail.AddErrors(error);
        return fail;
    }
    
    public static GeneralFail GeneralFail(IDictionary<string, List<string>> errors)
    {
        var fail = GeneralFail();
        fail.AddErrors(errors);
        return fail;
    }
    
    public static GeneralFail GeneralFail(Result r1)
        => GeneralFail(r1, Results.Success.Empty, Results.Success.Empty, Results.Success.Empty);
    
    public static GeneralFail GeneralFail(Result r1, Result r2)
        => GeneralFail(r1, r2, Results.Success.Empty, Results.Success.Empty);

    public static GeneralFail GeneralFail(Result r1, Result r2, Result r3)
        => GeneralFail(r1, r2, r3, Results.Success.Empty);

    /// <summary>
    /// Creates <see cref="BudgetCast.Common.Domain.Results.GeneralFail"/> result and merges
    /// errors from all the passed <see cref="Result"/> instances into it.
    /// </summary>
    /// <param name="r1">Result #1</param>
    /// <param name="r2">Result #2</param>
    /// <param name="r3">Result #3</param>
    /// <param name="r4">Result #4</param>
    /// <returns></returns>
    public static GeneralFail GeneralFail(Result r1, Result r2, Result r3, Result r4)
    {
        var fail = GeneralFail();
        
        if (!r1)
        {
            fail.AddErrors(r1.Errors);
        }

        if (!r2)
        {
            fail.AddErrors(r2.Errors);    
        }

        if (!r3)
        {
            fail.AddErrors(r3.Errors);    
        }

        if (!r4)
        {
            fail.AddErrors(r4.Errors);
        }
        
        return fail;
    }
    
    #endregion
    
    #region Creation methods - general fail - typed
    public static GeneralFail<T> GeneralFail<T>() 
        where T : notnull => new();
    
    public static GeneralFail<T> GeneralFail<T>(Error error)
        where T : notnull
    {
        var fail = GeneralFail<T>();
        fail.AddErrors(error);
        return fail;
    }

    public static GeneralFail<T> GeneralFail<T>(string error)
        where T : notnull
    {
        var fail = GeneralFail<T>();
        fail.AddErrors(error);
        return fail;
    }

    public static GeneralFail<T> GeneralFail<T>(IDictionary<string, List<string>> errors)
        where T : notnull
    {
        var fail = GeneralFail<T>();
        fail.AddErrors(errors);
        return fail;
    }

    public static GeneralFail<T> GeneralFail<T>(Result r1)
        where T : notnull => GeneralFail<T>(r1, Results.Success.Empty, Results.Success.Empty, Results.Success.Empty);
    
    public static GeneralFail<T> GeneralFail<T>(Result r1, Result r2)
        where T : notnull => GeneralFail<T>(r1, r2, Results.Success.Empty, Results.Success.Empty);

    public static GeneralFail<T> GeneralFail<T>(Result r1, Result r2, Result r3)
        where T : notnull => GeneralFail<T>(r1, r2, r3, Results.Success.Empty);

    /// <summary>
    /// Creates <see cref="BudgetCast.Common.Domain.Results.GeneralFail{T}"/> result and merges
    /// errors from all the passed <see cref="Result"/> instances into it.
    /// </summary>
    /// <param name="r1">Result #1</param>
    /// <param name="r2">Result #2</param>
    /// <param name="r3">Result #3</param>
    /// <param name="r4">Result #4</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static GeneralFail<T> GeneralFail<T>(Result r1, Result r2, Result r3, Result r4)
        where T : notnull
    {
        var fail = GeneralFail<T>();
        
        if (!r1)
        {
            fail.AddErrors(r1.Errors);
        }

        if (!r2)
        {
            fail.AddErrors(r2.Errors);    
        }

        if (!r3)
        {
            fail.AddErrors(r3.Errors);    
        }

        if (!r4)
        {
            fail.AddErrors(r4.Errors);
        }
        
        return fail;
    }
    
    #endregion
    
    #region Creation methods - invalid input
    public static InvalidInput InvalidInput() => new();
    public static InvalidInput InvalidInput(Error error)
    {
        var fail = InvalidInput();
        fail.AddErrors(error);
        return fail;
    }

    public static InvalidInput<T> InvalidInput<T>() where T : notnull => new();
    public static InvalidInput<T> InvalidInput<T>(Error error)
        where T : notnull
    {
        var fail = InvalidInput<T>();
        fail.AddErrors(error);
        return fail;
    }

    #endregion

    #region Creation methods - not found

    public static NotFound NotFound() => new();
    public static NotFound NotFound(Error error)
    {
        var fail = NotFound();
        fail.AddErrors(error);
        return fail;
    }
    
    public static NotFound<T> NotFound<T>() where T : notnull => new();
    public static NotFound<T> NotFound<T>(Error error)
        where T : notnull
    {
        var fail = NotFound<T>();
        fail.AddErrors(error);
        return fail;
    }

    #endregion

    #region Private members

    /// <summary>
    /// Checks if underlying type of current instance is either <see cref="BudgetCast.Common.Domain.Results.Success"/>
    /// or <see cref="BudgetCast.Common.Domain.Results.Success{T}"/>. 
    /// </summary>
    /// <returns></returns>
    private bool IsOfSuccessType()
    {
        var type = GetType();
        return type.IsAssignableTo(typeof(Success)) ||
               (type.IsGenericType && type.GetGenericTypeDefinition().IsAssignableTo(typeof(Success<>)));
    }

    #endregion
}
#endregion

#region Abstract typed Result
/// <summary>
/// Represented typed operation result.
/// </summary>
/// <typeparam name="T">Resulting operation data type</typeparam>
public abstract record Result<T> : Result
    where T : notnull
{
    private readonly T _value = default!;
    
    /// <summary>
    /// Operation result value.
    /// </summary>
    /// <exception cref="ResultValueIsNullException">Thrown when client tries to access value
    /// but it is set to <c>null</c></exception>
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

    /// <summary>
    /// Converts instances of <see cref="Error"/> type into <see cref="Result{T}"/>
    /// with underlying <see cref="BudgetCast.Common.Domain.Results.GeneralFail{T}"/> type.
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public static implicit operator Result<T>(Error error) => GeneralFail<T>(error);
    
    /// <summary>
    /// Converts instances of <typeparamref name="T"/> into <see cref="BudgetCast.Common.Domain.Results.Success{T}"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
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
    where T : notnull
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
    where T : notnull
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
    where T : notnull
{
    public static implicit operator NotFound<T>(Error error) => Result.NotFound<T>(error);
}

#endregion