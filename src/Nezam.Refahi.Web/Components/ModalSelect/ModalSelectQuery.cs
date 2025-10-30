using System.ComponentModel.DataAnnotations;

namespace Nezam.New.EES.Components.ModalSelect;

/// <summary>
/// Incoming query used by every lookup endpoint.  
/// <para>The four *Field* properties let a single endpoint serve many pickers:
///     `valueField="id"` `textField="fullName"` ...</para>
/// </summary>
public record ModalSelectQuery
{
    /* ---------- paging ------------------------------- */
    [Range(1, 255)]
    public int Page { get; init; } = 1;

    [Range(1, 100)]
    public int PageSize { get; init; } = 10;

    /* ---------- search ------------------------------- */
    public string? Search { get; init; }

    /* ---------- mapping ------------------------------ */
    public string ValueField       { get; init; } = "id";
    public string TextField        { get; init; } = "text";
    public string? DescriptionField{ get; init; }   // optional
    /// <summary>Comma-separated list of additional columns the front-end wants back.</summary>
    public string? ExtraFields     { get; init; }

    /* ---------- arbitrary extra filters -------------- */
    /// <example>&amp;filters[roleId]=3&amp;filters[status]=active</example>
    public Dictionary<string,string>? Filters { get; init; }
}

/// <summary>A single row in <see cref="ModalSelectResponse.Items"/>.</summary>
public record ModalSelectItem
{
    public required string Id          { get; init; }  // what gets posted back
    public required string Text        { get; init; }  // main display
    public string?       Description   { get; init; }  // secondary line / tooltip
    public Dictionary<string,object?>? Extras { get; init; } // any other columns
}

/// <summary>
/// Fixed shape every lookup API returns.  
/// The front-end relies on these exact property names (camel-cased in JSON).
/// </summary>
public record ModalSelectResponse
{
    public required IReadOnlyList<ModalSelectItem> Items { get; init; }

    /* ---- paging echoes ---- */
    public  int CurrentPage { get; init; } = 0;
    public  int PageSize { get; init; } = 0;

    /* ---- totals ---- */
    public  int TotalItems { get; init; } = 0;
    public  int TotalPages { get; init; } = 0;

    /* ---- optional echo of the query (handy for debugging or lazy-loading) ---- */
    public ModalSelectQuery? QueryEcho { get; init; }
}
