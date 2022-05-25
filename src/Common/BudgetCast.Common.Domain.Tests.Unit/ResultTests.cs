using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using BudgetCast.Common.Domain.Results;
using BudgetCast.Common.Domain.Results.Exceptions;
using FluentAssertions;
using Xunit;

namespace BudgetCast.Common.Domain.Tests.Unit;

public class ResultTests
{
    private readonly ResultFixture _fixture;

    public ResultTests()
    {
        _fixture = new ResultFixture();
    }

    #region Public contract properties

    [MemberData(nameof(ResultFixture.GetNonSuccessResultInstances), MemberType = typeof(ResultFixture))]
    [Theory]
    public void IsOfFailure_Any_Non_SuccessType_Should_Return_True(Result result)
    {
        // Arrange
        
        // Act
        
        // Assert
        result.IsOfFailure.Should().BeTrue();
    }
    
    [MemberData(nameof(ResultFixture.GetSuccessResultInstances), MemberType = typeof(ResultFixture))]
    [Theory]
    public void IsOfFailure_Any_SuccessType_Should_Return_False(Result result)
    {
        // Arrange
        
        // Act
        
        // Assert
        result.IsOfFailure.Should().BeFalse();
    }

    [Fact]
    public void IsOfFailure_Untyped_Success_Type_With_Non_Empty_Errors_Collection_Should_Return_True()
    {
        // Arrange
        var errors = new Dictionary<string, List<string>>();
        errors.Add(_fixture.Fixture.Create<string>(), _fixture.Fixture.Create<List<string>>());
        
        var result = new Success
        {
            Errors = errors,
        };

        // Act
        
        // Assert
        result.IsOfFailure.Should().BeTrue();
    }
    
    [Fact]
    public void IsOfFailure_Typed_Success_Type_With_Non_Empty_Errors_Collection_Should_Return_True()
    {
        // Arrange
        var errors = new Dictionary<string, List<string>>();
        errors.Add(_fixture.Fixture.Create<string>(), _fixture.Fixture.Create<List<string>>());
        
        var result = new Success<FakeData>(new FakeData())
        {
            Errors = errors,
        };

        // Act
        
        // Assert
        result.IsOfFailure.Should().BeTrue();
    }

    #endregion

    #region Public contract methods

    [MemberData(nameof(ResultFixture.GetSuccessResultInstances), MemberType = typeof(ResultFixture))]
    [Theory]
    public void AddErrors_StringError_Should_Throw_If_Adding_To_SuccessType(Result result)
    {
        // Arrange
        
        // Act
        Action addError = () =>
        {
            result.AddErrors(_fixture.Fixture.Create<string>());
        };

        // Assert
        Assert.Throws<AddingErrorsToSuccessResultException>(() => addError());
    }

    [MemberData(nameof(ResultFixture.GetNonSuccessResultInstances), MemberType = typeof(ResultFixture))]
    [Theory]
    public void AddErrors_StringError_Should_Create_Records_In_Errors_HashTable_Under_GeneralKey(Result result)
    {
        // Arrange
        var errorMessage1 = _fixture.Fixture.Create<string>();
        var errorMessage2 = _fixture.Fixture.Create<string>();
        
        // Act
        result.AddErrors(errorMessage1);
        result.AddErrors(errorMessage2);

        // Assert
        result.Errors["general"].Should().Contain(d => d == errorMessage1);
        result.Errors["general"].Should().Contain(d => d == errorMessage2);
    }
    
    [MemberData(nameof(ResultFixture.GetSuccessResultInstances), MemberType = typeof(ResultFixture))]
    [Theory]
    public void AddErrors_ErrorType_Should_Throw_If_Adding_To_SuccessType(Result result)
    {
        // Arrange
        
        // Act
        Action addError = () =>
        {
            result.AddErrors(new Error(_fixture.Fixture.Create<string>(), _fixture.Fixture.Create<string>()));
        };

        // Assert
        Assert.Throws<AddingErrorsToSuccessResultException>(() => addError());
    }

    [MemberData(nameof(ResultFixture.GetNonSuccessResultInstances), MemberType = typeof(ResultFixture))]
    [Theory]
    public void AddErrors_ErrorType_Should_Create_Records_In_Errors_HashTable_Under_Error_Key(Result result)
    {
        // Arrange
        var error1 = new Error(_fixture.Fixture.Create<string>(), _fixture.Fixture.Create<string>());
        var error2 = new Error(_fixture.Fixture.Create<string>(), _fixture.Fixture.Create<string>());
        
        // Act
        result.AddErrors(error1);
        result.AddErrors(error2);

        // Assert
        result.Errors[error1.Code].Should().Contain(d => d == error1.Value);
        result.Errors[error2.Code].Should().Contain(d => d == error2.Value);
    }
    
    [MemberData(nameof(ResultFixture.GetSuccessResultInstances), MemberType = typeof(ResultFixture))]
    [Theory]
    public void AddErrors_ErrorsHashTable_Should_Throw_If_Adding_To_SuccessType(Result result)
    {
        // Arrange
        
        // Act
        Action addError = () =>
        {
            result.AddErrors(_fixture.Fixture.Create<IDictionary<string, List<string>>>());
        };

        // Assert
        Assert.Throws<AddingErrorsToSuccessResultException>(() => addError());
    }

    [MemberData(nameof(ResultFixture.GetNonSuccessResultInstances), MemberType = typeof(ResultFixture))]
    [Theory]
    public void AddErrors_ErrorsHashTable_Should_Create_Records_In_Errors_HashTable_Under_Error_Key(Result result)
    {
        // Arrange
        var errorHashTable1 = _fixture.Fixture.Create<IDictionary<string, List<string>>>();
        var errorHashTable2 = _fixture.Fixture.Create<IDictionary<string, List<string>>>();

        var allErrorsKeys = errorHashTable1.Keys.Concat(errorHashTable2.Keys);
        
        // Act
        result.AddErrors(errorHashTable1);
        result.AddErrors(errorHashTable2);

        // Assert
        result.Errors.Keys
            .Should()
            .BeEquivalentTo(allErrorsKeys);
    }

    #endregion
    
    #region Implicit conversion

    [Fact]
    public void Implicit_Conversion_From_ErrorType_ShouldCreate_Untyped_GeneralFail()
    {
        // Arrange
        static Result FromError(Error error)
            => error;
        
        var error = new Error(_fixture.Fixture.Create<string>(), _fixture.Fixture.Create<string>());
        
        // Act
        var result = FromError(error);

        // Assert
        result.Should().BeOfType<GeneralFail>();
    }

    [MemberData(nameof(ResultFixture.GetNonSuccessResultInstances), MemberType = typeof(ResultFixture))]
    [Theory]
    public void Implicit_Conversion_From_Result_To_Boolean_Should_Return_False_For_Any_Failure_Type(Result result)
    {
        // Arrange
        var conversionResult = true;
        
        // Act
        conversionResult = result;

        // Assert
        conversionResult.Should().BeFalse();
    }
    
    [MemberData(nameof(ResultFixture.GetSuccessResultInstances), MemberType = typeof(ResultFixture))]
    [Theory]
    public void Implicit_Conversion_From_Result_To_Boolean_Should_Return_True_For_Any_Success_Type(Result result)
    {
        // Arrange
        var conversionResult = false;
        
        // Act
        conversionResult = result;

        // Assert
        conversionResult.Should().BeTrue();
    }
    
    #endregion
    
    #region Creation methods - success

    [Fact]
    public void Success_Should_Return_New_Instance_Of_Untyped_Success_Type()
    {
        // Arrange
        
        // Act
        var result = Result.Success();

        // Assert
        result.Should().BeOfType<Success>();
    }
    
    [Fact]
    public void Success_Should_Return_New_Instance_Of_Typed_Success_Type()
    {
        // Arrange
        var data = new FakeData();
        
        // Act
        var result = Result.Success(data);

        // Assert
        result.Should().BeOfType<Success<FakeData>>();
        result.Value.Should().BeEquivalentTo(data);
    }
    
    [Fact]
    public void Success_Null_As_Data_Should_Throw_Exception()
    {
        // Arrange

        // Act
        var action = () =>
        {
            _ = Result.Success<FakeData>(null!);
        };

        // Assert
        Assert.Throws<ResultValueIsNullException>(action);
    }
    
    #endregion
    
    #region Creation methods - general fail - untyped

    [Fact]
    public void GeneralFail_Should_Return_New_Instance_Of_Untyped_GeneralFail_Type()
    {
        // Arrange
        
        // Act
        var result = Result.GeneralFail();

        // Assert
        result.Should().BeOfType<GeneralFail>();
    }

    [Fact]
    public void GeneralFail_ErrorType_Should_Create_New_Instance_Of_Untyped_GeneralFail_Type_With_Errors()
    {
        // Arrange
        var error = new Error(_fixture.Fixture.Create<string>(), _fixture.Fixture.Create<string>());
        
        // Act
        var result = Result.GeneralFail(error);

        // Assert
        result.Should().BeOfType<GeneralFail>();
        result.Errors[error.Code].Should().Contain(v => v == error.Value);
    }
    
    [Fact]
    public void GeneralFail_ErrorHashTable_Should_Create_New_Instance_Of_Untyped_GeneralFail_Type_With_Errors()
    {
        // Arrange
        var errorHashTable = _fixture.Fixture.Create<IDictionary<string, List<string>>>();
        
        // Act
        var result = Result.GeneralFail(errorHashTable);

        // Assert
        result.Should().BeOfType<GeneralFail>();
        result.Errors.Keys
            .Should()
            .BeEquivalentTo(errorHashTable.Keys);
    }

    [Fact]
    public void GeneralFailOf_4_FailedResults_Should_Return_New_Instance_Of_Untyped_GeneralFail_With_Errors_From_Passed_Results()
    {
        // Arrange
        var e1 = new Error(_fixture.Fixture.Create<string>(), _fixture.Fixture.Create<string>());
        var r1 = Result.GeneralFail(e1);
        
        var e2 = new Error(_fixture.Fixture.Create<string>(), _fixture.Fixture.Create<string>());
        var r2 = Result.GeneralFail(e2);
        
        var e3 = new Error(_fixture.Fixture.Create<string>(), _fixture.Fixture.Create<string>());
        var r3 = Result.GeneralFail(e3);
        
        var e4 = new Error(_fixture.Fixture.Create<string>(), _fixture.Fixture.Create<string>());
        var r4 = Result.GeneralFail(e4);

        // Act
        var result = Result.GeneralFailOf(r1, r2, r3, r4);

        // Assert
        result.Should().BeOfType<GeneralFail>();
        result.Errors.Keys.Should().Contain(r1.Errors.Keys);
        result.Errors.Keys.Should().Contain(r2.Errors.Keys);
        result.Errors.Keys.Should().Contain(r3.Errors.Keys);
        result.Errors.Keys.Should().Contain(r4.Errors.Keys);
    }
    
    [Fact]
    public void GeneralFailOf_4_SuccessResults_Should_Return_New_Instance_Of_Untyped_GeneralFail_With_No_Errors()
    {
        // Arrange
        var r1 = Result.Success();
        var r2 = Result.Success();
        var r3 = Result.Success();
        var r4 = Result.Success();

        // Act
        var result = Result.GeneralFailOf(r1, r2, r3, r4);

        // Assert
        result.Should().BeOfType<GeneralFail>();
        result.Errors.Keys.Should().BeEmpty();
    }
    
    #endregion
    
    #region Creation methods - general fail - typed

    [Fact]
    public void GeneralFail_Should_Return_New_Instance_Of_Typed_GeneralFail_Type()
    {
        // Arrange
        
        // Act
        var result = Result.GeneralFail<FakeData>();

        // Assert
        result.Should().BeOfType<GeneralFail<FakeData>>();
    }

    [Fact]
    public void GeneralFail_ErrorType_Should_Create_New_Instance_Of_Typed_GeneralFail_Type_With_Errors()
    {
        // Arrange
        var error = new Error(_fixture.Fixture.Create<string>(), _fixture.Fixture.Create<string>());
        
        // Act
        var result = Result.GeneralFail<FakeData>(error);

        // Assert
        result.Should().BeOfType<GeneralFail<FakeData>>();
        result.Errors[error.Code].Should().Contain(v => v == error.Value);
    }
    
    [Fact]
    public void GeneralFail_ErrorHashTable_Should_Create_New_Instance_Of_Typed_GeneralFail_Type_With_Errors()
    {
        // Arrange
        var errorHashTable = _fixture.Fixture.Create<IDictionary<string, List<string>>>();
        
        // Act
        var result = Result.GeneralFail<FakeData>(errorHashTable);

        // Assert
        result.Should().BeOfType<GeneralFail<FakeData>>();
        result.Errors.Keys
            .Should()
            .BeEquivalentTo(errorHashTable.Keys);
    }

    [Fact]
    public void GeneralFailOf_4_FailedResults_Should_Return_New_Instance_Of_Typed_GeneralFail_With_Errors_From_Passed_Results()
    {
        // Arrange
        var e1 = new Error(_fixture.Fixture.Create<string>(), _fixture.Fixture.Create<string>());
        var r1 = Result.GeneralFail<FakeData>(e1);
        
        var e2 = new Error(_fixture.Fixture.Create<string>(), _fixture.Fixture.Create<string>());
        var r2 = Result.GeneralFail<FakeData>(e2);
        
        var e3 = new Error(_fixture.Fixture.Create<string>(), _fixture.Fixture.Create<string>());
        var r3 = Result.GeneralFail<FakeData>(e3);
        
        var e4 = new Error(_fixture.Fixture.Create<string>(), _fixture.Fixture.Create<string>());
        var r4 = Result.GeneralFail<FakeData>(e4);

        // Act
        var result = Result.GeneralFailOf<FakeData>(r1, r2, r3, r4);

        // Assert
        result.Should().BeOfType<GeneralFail<FakeData>>();
        result.Errors.Keys.Should().Contain(r1.Errors.Keys);
        result.Errors.Keys.Should().Contain(r2.Errors.Keys);
        result.Errors.Keys.Should().Contain(r3.Errors.Keys);
        result.Errors.Keys.Should().Contain(r4.Errors.Keys);
    }
    
    [Fact]
    public void GeneralFailOf_4_SuccessResults_Should_Return_New_Instance_Of_Typed_GeneralFail_With_No_Errors()
    {
        // Arrange
        var r1 = Result.Success(new FakeData());
        var r2 = Result.Success(new FakeData());
        var r3 = Result.Success(new FakeData());
        var r4 = Result.Success(new FakeData());

        // Act
        var result = Result.GeneralFailOf<FakeData>(r1, r2, r3, r4);

        // Assert
        result.Should().BeOfType<GeneralFail<FakeData>>();
        result.Errors.Keys.Should().BeEmpty();
    }
    
    #endregion

    private sealed class ResultFixture
    {
        public Fixture Fixture { get; }

        public ResultFixture()
        {
            Fixture = new Fixture();
        }
        
        public static IEnumerable<object[]> GetInstancesOfAllResultTypes()
            => GetSuccessResultInstances().Concat(GetNonSuccessResultInstances());

        public static IEnumerable<object[]> GetSuccessResultInstances()
        {
            yield return new object[] { Result.Success() };
            yield return new object[] { Result.Success(new FakeData()) };
        }
        
        public static IEnumerable<object[]> GetNonSuccessResultInstances()
        {
            yield return new object[] { Result.GeneralFail() };
            yield return new object[] { Result.GeneralFail<FakeData>() };
            yield return new object[] { Result.InvalidInput() };
            yield return new object[] { Result.InvalidInput<FakeData>() };
            yield return new object[] { Result.NotFound() };
            yield return new object[] { Result.NotFound<FakeData>() };
        }
    }
    
    public record FakeData;
}