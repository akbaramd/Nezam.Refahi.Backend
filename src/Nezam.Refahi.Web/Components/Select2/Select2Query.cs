// File: Common/Select2/Select2Query.cs

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Nezam.New.EES.Components.Select2;

/// <summary>
/// Represents the query-string contract expected by ALL Select2 AJAX endpoints:
/// ?q=search&page=1&pageSize=20&extra[key]=value
/// </summary>
public sealed class Select2Query : IValidatableObject
{
    public const int DefaultPageSize = 20;
    public const int MaxPageSize     = 100;

    /// <summary>Search term (empty = “match all”).</summary>
    [FromQuery(Name = "q")]
    public string? Term { get; init; }

    /// <summary>1-based page index (Select2 uses `page`).</summary>
    [Range(1, int.MaxValue)]
    [FromQuery(Name = "page")]
    public int Page { get; init; } = 1;

    /// <summary>Items per page.</summary>
    [Range(1, MaxPageSize)]
    [FromQuery(Name = "pageSize")]
    public int PageSize { get; init; } = DefaultPageSize;

    /// <summary>
    /// Optional additional key/value pairs forwarded unchanged to the server.
    /// Select2 sends them as "extra[key]=value".
    /// </summary>
    [FromQuery(Name = "extra")]
    public IDictionary<string, string>? Extra { get; init; }

    // ---------------------------------------------------------------------
    //  IValidatableObject – guards against malicious or broken requests
    // ---------------------------------------------------------------------
    public IEnumerable<ValidationResult> Validate(ValidationContext ctx)
    {
        if (PageSize > MaxPageSize)
        {
            yield return new ValidationResult(
                $"pageSize cannot exceed {MaxPageSize}.",
                new[] { nameof(PageSize) });
        }
    }

    // ---------------------------------------------------------------------
    //  Convenience helpers
    // ---------------------------------------------------------------------

    /// <summary>
    /// Calculates how many rows to <c>Skip</c> when querying the DB.
    /// </summary>
    public int Skip => (Page - 1) * PageSize;

    /// <summary>
    /// Re-build this request into a query-string you can append to a URL.
    /// </summary>
    public string ToQueryString()
    {
        var dict = new Dictionary<string, string?>  // nulls are ignored by QueryHelpers
        {
            ["q"]        = Term,
            ["page"]     = Page.ToString(),
            ["pageSize"] = PageSize.ToString()
        };

        if (Extra is { Count: >0 })
        {
            foreach (var (k, v) in Extra)
                dict[$"extra[{k}]"] = v;
        }

        return QueryHelpers.AddQueryString(string.Empty, dict);
    }

    /// <summary>
    /// Fast factory for unit tests or manual construction.
    /// </summary>
    public static Select2Query Create(string? q = null, int page = 1, int pageSize = DefaultPageSize,
                                      IDictionary<string,string>? extra = null) =>
        new() { Term = q, Page = page, PageSize = pageSize, Extra = extra };
}
