using Microsoft.AspNetCore.Http;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Presentation.Extensions;

/// <summary>
/// Extension methods for ApplicationResult to map to HTTP status codes
/// </summary>
public static class ApplicationResultExtensions
{
    /// <summary>
    /// Maps ApplicationResult to appropriate HTTP status code result
    /// </summary>
    public static IResult ToResult(this ApplicationResult result)
    {
        if (result.IsSuccess)
            return Results.Ok(result);

        return result.Status switch
        {
            ResultStatus.BadRequest => Results.BadRequest(result),
            ResultStatus.Unauthorized => Results.Unauthorized(),
            ResultStatus.Forbidden => Results.StatusCode(403),
            ResultStatus.NotFound => Results.NotFound(result),
            ResultStatus.Conflict => Results.Conflict(result),
            ResultStatus.Gone => Results.StatusCode(410),
            ResultStatus.ValidationFailed => Results.UnprocessableEntity(result),
            ResultStatus.RateLimited => Results.StatusCode(429),
            ResultStatus.InternalError => Results.StatusCode(500),
            _ => Results.BadRequest(result)
        };
    }

    /// <summary>
    /// Maps ApplicationResult&lt;T&gt; to appropriate HTTP status code result
    /// </summary>
    public static IResult ToResult<T>(this ApplicationResult<T> result)
    {
        if (result.IsSuccess)
            return Results.Ok(result);

        return result.Status switch
        {
            ResultStatus.BadRequest => Results.BadRequest(result),
            ResultStatus.Unauthorized => Results.Unauthorized(),
            ResultStatus.Forbidden => Results.StatusCode(403),
            ResultStatus.NotFound => Results.NotFound(result),
            ResultStatus.Conflict => Results.Conflict(result),
            ResultStatus.Gone => Results.StatusCode(410),
            ResultStatus.ValidationFailed => Results.UnprocessableEntity(result),
            ResultStatus.RateLimited => Results.StatusCode(429),
            ResultStatus.InternalError => Results.StatusCode(500),
            _ => Results.BadRequest(result)
        };
    }
}

/// <summary>
/// Extension methods for Results class to handle ApplicationResult with automatic status code mapping
/// </summary>
public static class ResultsExtensions
{
    /// <summary>
    /// Returns an HTTP result based on ApplicationResult status, automatically handling the status code
    /// </summary>
    /// <param name="result">The application result to convert</param>
    /// <returns>IResult with appropriate HTTP status code</returns>
    public static IResult FromApplicationResult(ApplicationResult result)
    {
        if (result.IsSuccess)
            return Results.Ok(result);

        return result.Status switch
        {
            ResultStatus.BadRequest => Results.BadRequest(result),
            ResultStatus.Unauthorized => Results.Unauthorized(),
            ResultStatus.Forbidden => Results.StatusCode(403),
            ResultStatus.NotFound => Results.NotFound(result),
            ResultStatus.Conflict => Results.Conflict(result),
            ResultStatus.Gone => Results.StatusCode(410),
            ResultStatus.ValidationFailed => Results.UnprocessableEntity(result),
            ResultStatus.RateLimited => Results.StatusCode(429),
            ResultStatus.InternalError => Results.StatusCode(500),
            _ => Results.BadRequest(result)
        };
    }

    /// <summary>
    /// Returns an HTTP result based on ApplicationResult&lt;T&gt; status, automatically handling the status code
    /// </summary>
    /// <typeparam name="T">Type of the result data</typeparam>
    /// <param name="result">The application result to convert</param>
    /// <returns>IResult with appropriate HTTP status code</returns>
    public static IResult FromApplicationResult<T>(ApplicationResult<T> result)
    {
        if (result.IsSuccess)
            return Results.Ok(result);

        return result.Status switch
        {
            ResultStatus.BadRequest => Results.BadRequest(result),
            ResultStatus.Unauthorized => Results.Unauthorized(),
            ResultStatus.Forbidden => Results.StatusCode(403),
            ResultStatus.NotFound => Results.NotFound(result),
            ResultStatus.Conflict => Results.Conflict(result),
            ResultStatus.Gone => Results.StatusCode(410),
            ResultStatus.ValidationFailed => Results.UnprocessableEntity(result),
            ResultStatus.RateLimited => Results.StatusCode(429),
            ResultStatus.InternalError => Results.StatusCode(500),
            _ => Results.BadRequest(result)
        };
    }

    /// <summary>
    /// Returns an HTTP result based on ApplicationResult status with custom handling
    /// </summary>
    /// <param name="result">The application result to convert</param>
    /// <param name="onSuccess">Optional custom handler for success case</param>
    /// <param name="onFailure">Optional custom handler for failure case</param>
    /// <returns>IResult with appropriate HTTP status code</returns>
    public static IResult FromApplicationResult(
        ApplicationResult result,
        Func<ApplicationResult, IResult>? onSuccess = null,
        Func<ApplicationResult, IResult>? onFailure = null)
    {
        if (result.IsSuccess)
            return onSuccess?.Invoke(result) ?? Results.Ok(result);

        if (onFailure != null)
            return onFailure(result);

        return result.Status switch
        {
            ResultStatus.BadRequest => Results.BadRequest(result),
            ResultStatus.Unauthorized => Results.Unauthorized(),
            ResultStatus.Forbidden => Results.StatusCode(403),
            ResultStatus.NotFound => Results.NotFound(result),
            ResultStatus.Conflict => Results.Conflict(result),
            ResultStatus.Gone => Results.StatusCode(410),
            ResultStatus.ValidationFailed => Results.UnprocessableEntity(result),
            ResultStatus.RateLimited => Results.StatusCode(429),
            ResultStatus.InternalError => Results.StatusCode(500),
            _ => Results.BadRequest(result)
        };
    }

    /// <summary>
    /// Returns an HTTP result based on ApplicationResult&lt;T&gt; status with custom handling
    /// </summary>
    /// <typeparam name="T">Type of the result data</typeparam>
    /// <param name="result">The application result to convert</param>
    /// <param name="onSuccess">Optional custom handler for success case</param>
    /// <param name="onFailure">Optional custom handler for failure case</param>
    /// <returns>IResult with appropriate HTTP status code</returns>
    public static IResult FromApplicationResult<T>(
        ApplicationResult<T> result,
        Func<ApplicationResult<T>, IResult>? onSuccess = null,
        Func<ApplicationResult<T>, IResult>? onFailure = null)
    {
        if (result.IsSuccess)
            return onSuccess?.Invoke(result) ?? Results.Ok(result);

        if (onFailure != null)
            return onFailure(result);

        return result.Status switch
        {
            ResultStatus.BadRequest => Results.BadRequest(result),
            ResultStatus.Unauthorized => Results.Unauthorized(),
            ResultStatus.Forbidden => Results.StatusCode(403),
            ResultStatus.NotFound => Results.NotFound(result),
            ResultStatus.Conflict => Results.Conflict(result),
            ResultStatus.Gone => Results.StatusCode(410),
            ResultStatus.ValidationFailed => Results.UnprocessableEntity(result),
            ResultStatus.RateLimited => Results.StatusCode(429),
            ResultStatus.InternalError => Results.StatusCode(500),
            _ => Results.BadRequest(result)
        };
    }
}

