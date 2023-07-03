using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.ObjectModel;
using System.Linq;

namespace Recipe.Web.Application.Features.Shared;

public record Error(string PropertyName, string ErrorMessage);

public interface IResult
{
    public int StatusCode { get; }

    public bool WasSuccessful { get; }

    public void AddError(Error error);
}

public record Result(int StatusCode) : IResult
{
    public bool WasSuccessful => StatusCode >= 200 && StatusCode <= 299;

    public bool IsBadRequest => StatusCode == 400;

    public bool HasErrors => errors.Any();

    private readonly List<Error> errors = new List<Error>();

    public ReadOnlyCollection<Error> Errors => errors.AsReadOnly();

    public void AddError(Error error) => errors.Add(error);

    public Result(int StatusCode, Error error) : this(StatusCode) => AddError(error);

    public Result(int StatusCode, IEnumerable<Error> errors) : this(StatusCode)
    {
        foreach (var error in errors)
        {
            AddError(error);
        }
    }

    public static Result Ok() => new Result(200);

    public static Result<T> Ok<T>(T value) => new Result<T>(200, value);

    public static Result BadRequest() => new Result(400);

    public static Result BadRequest(Error error) => new Result(400, error);

    public static Result BadRequest(IEnumerable<Error> errors) => new Result(400, errors);

    public static Result<T> BadRequest<T>(Error error) => new Result<T>(400, error);

    public static Result NotFound() => new Result(404);

    public static Result<T> NotFound<T>() => new Result<T>(404);

    public static Result ServerError() => new Result(500);

    public static Result<T> ServerError<T>() => new Result<T>(500);
}

public record Result<T>(int StatusCode) : Result(StatusCode)
{
    private readonly T value = default;

    public T Value => WasSuccessful ? value : throw new InvalidOperationException("Unable to get value of an unsuccessful Result.");

    public Result(int StatusCode, T Value) : this(StatusCode) => value = Value;

    public Result(int StatusCode, Error error) : this(StatusCode) => AddError(error);
}

public static class ResultExtensions
{
    public static void AddErrors(this ModelStateDictionary modelState, Result result)
    {
        if (modelState == null || result == null)
        {
            return;
        }

        foreach (var error in result.Errors)
        {
            modelState.TryAddModelError(error.PropertyName, error.ErrorMessage);
        }
    }
}
