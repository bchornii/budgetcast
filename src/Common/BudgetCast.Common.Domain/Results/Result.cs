using BudgetCast.Common.Domain.Results.Exceptions;

namespace BudgetCast.Common.Domain.Results;

/// <summary>
/// Represents untyped operation result.
/// </summary>
public abstract record Result
{
    public const string NotClassifiedApplicationError = "app.general";
    
    #region Public contract properties

    /// <summary>
    /// Verifies if underlying type of <see cref="Result"/> if of failure type.
    /// </summary>
    public bool IsOfFailure => HasErrors || !IsOfSuccessType();

    /// <summary>
    /// Verifies if underlying type has any errors added.
    /// </summary>
    public bool HasErrors => Errors.Count > 0;
    
    /// <summary>
    /// Represents errors hash table associated with an instance of <see cref="Result"/> type.
    /// </summary>
    public IDictionary<string, List<string>> Errors { get; init; } = new Dictionary<string, List<string>>();

    #endregion

    #region Public contract methods

    /// <summary>
    /// Adds an error to <see cref="Result"/> instance's errors hash table with
    /// value of <paramref name="error"/> passed parameter under <see cref="NotClassifiedApplicationError"/>.
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

        if (Errors.ContainsKey(NotClassifiedApplicationError))
        {
            Errors[NotClassifiedApplicationError].Add(error);
        }
        else
        {
            Errors.Add(NotClassifiedApplicationError, new List<string> { error });
        }

        return this;
    }
    
    /// <summary>
    /// Adds an error to <see cref="Result"/> instance's errors hash table with
    /// key <see cref="ValidationError.Code"/> and value of <see cref="ValidationError.Value"/>.
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    /// <exception cref="AddingErrorsToSuccessResultException"></exception>
    public Result AddErrors(ValidationError error)
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
    /// Converts instances of <see cref="ValidationError"/> type into <see cref="Result"/>
    /// with underlying <see cref="BudgetCast.Common.Domain.Results.GeneralFail"/> type.
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public static implicit operator Result(ValidationError error) => GeneralFail(error);
    
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

    #region Creation methods - maybe

    public static Maybe<T> Maybe<T>(T value)
        => new(value);

    #endregion
    
    #region Creation methods - general fail - untyped

    public static GeneralFail GeneralFail() => new();
    
    public static GeneralFail GeneralFail(ValidationError error)
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
    
    public static GeneralFail GeneralFailOf(Result r1)
        => GeneralFailOf(r1, Results.Success.Empty, Results.Success.Empty, Results.Success.Empty);
    
    public static GeneralFail GeneralFailOf(Result r1, Result r2)
        => GeneralFailOf(r1, r2, Results.Success.Empty, Results.Success.Empty);

    public static GeneralFail GeneralFailOf(Result r1, Result r2, Result r3)
        => GeneralFailOf(r1, r2, r3, Results.Success.Empty);

    /// <summary>
    /// Creates <see cref="BudgetCast.Common.Domain.Results.GeneralFail"/> result and merges
    /// errors from all the passed <see cref="Result"/> instances into it.
    /// </summary>
    /// <param name="r1">Result #1</param>
    /// <param name="r2">Result #2</param>
    /// <param name="r3">Result #3</param>
    /// <param name="r4">Result #4</param>
    /// <returns></returns>
    public static GeneralFail GeneralFailOf(Result r1, Result r2, Result r3, Result r4)
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
    
    public static GeneralFail<T> GeneralFail<T>(ValidationError error)
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

    public static GeneralFail<T> GeneralFailOf<T>(Result r1)
        where T : notnull => GeneralFailOf<T>(r1, Results.Success.Empty, Results.Success.Empty, Results.Success.Empty);
    
    public static GeneralFail<T> GeneralFailOf<T>(Result r1, Result r2)
        where T : notnull => GeneralFailOf<T>(r1, r2, Results.Success.Empty, Results.Success.Empty);

    public static GeneralFail<T> GeneralFailOf<T>(Result r1, Result r2, Result r3)
        where T : notnull => GeneralFailOf<T>(r1, r2, r3, Results.Success.Empty);

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
    public static GeneralFail<T> GeneralFailOf<T>(Result r1, Result r2, Result r3, Result r4)
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
    public static InvalidInput InvalidInput(ValidationError error)
    {
        var fail = InvalidInput();
        fail.AddErrors(error);
        return fail;
    }

    public static InvalidInput<T> InvalidInput<T>() 
        where T : notnull => new();
    public static InvalidInput<T> InvalidInput<T>(ValidationError error)
        where T : notnull
    {
        var fail = InvalidInput<T>();
        fail.AddErrors(error);
        return fail;
    }

    #endregion

    #region Creation methods - not found

    public static NotFound NotFound() => new();
    public static NotFound NotFound(ValidationError error)
    {
        var fail = NotFound();
        fail.AddErrors(error);
        return fail;
    }
    
    public static NotFound<T> NotFound<T>() 
        where T : notnull => new();
    public static NotFound<T> NotFound<T>(ValidationError error)
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
               (type.IsGenericType && type.GetGenericTypeDefinition().IsAssignableTo(typeof(Success<>))) ||
               (type.IsGenericType && type.GetGenericTypeDefinition().IsAssignableTo(typeof(Maybe<>)));
    }

    #endregion
}

/// <summary>
/// Represented typed operation result.
/// </summary>
/// <typeparam name="T">Resulting operation data type</typeparam>
public abstract record Result<T> : Result
    where T : notnull
{
    private readonly T _value = default!;
    private readonly bool _isValueSet;
    
    /// <summary>
    /// Operation result value.
    /// </summary>
    /// <exception cref="ResultValueIsNullException">Thrown when client tries to access value
    /// but it is set to <c>null</c></exception>
    public virtual T Value
    {
        get
        {
            if (!_isValueSet || _value is null)
            {
                throw new ResultValueIsNullException(Errors);
            }

            return _value;
        }

        protected init
        {
            if (value is null)
            {
                throw new ResultValueIsNullException(Errors);
            }

            _value = value;
            _isValueSet = true;
        }
    }

    /// <summary>
    /// Converts instances of <see cref="ValidationError"/> type into <see cref="Result{T}"/>
    /// with underlying <see cref="BudgetCast.Common.Domain.Results.GeneralFail{T}"/> type.
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public static implicit operator Result<T>(ValidationError error) => GeneralFail<T>(error);
    
    /// <summary>
    /// Converts instances of <typeparamref name="T"/> into <see cref="BudgetCast.Common.Domain.Results.Success{T}"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator Result<T>(T value) => Success(value);
}