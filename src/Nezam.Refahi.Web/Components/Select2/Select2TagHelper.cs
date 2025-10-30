using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Nezam.New.EES.Components.Select2;

[HtmlTargetElement("select", Attributes = "asp-select2")]
public sealed class Select2TagHelper : SelectTagHelper
{
    public Select2TagHelper(IHtmlGenerator g) : base(g) { }

    // ─── public attributes (camel-cased like Select2’s own) ────────────
    [HtmlAttributeName("s2-url")]           public string  DataUrl          { get; set; } = default!;
    [HtmlAttributeName("s2-extra-params")]  public object? ExtraParams      { get; set; }

    [HtmlAttributeName("page-size")]             public int  PageSize       { get; set; } = 20;
    [HtmlAttributeName("minimum-input-length")]  public int  MinInputLength { get; set; }
    [HtmlAttributeName("allow-clear")]           public bool AllowClear     { get; set; }
    [HtmlAttributeName("multiple")]              public bool Multiple       { get; set; }
    [HtmlAttributeName("tags")]                  public bool Tags           { get; set; }
    [HtmlAttributeName("placeholder")]           public string? Placeholder { get; set; }
    [HtmlAttributeName("width")]                 public string? Width       { get; set; }

    // parent → child dependency
    [HtmlAttributeName("depends-on")]     public string? DependsOnSelector { get; set; }
    [HtmlAttributeName("depends-param")]  public string  DependsParam      { get; set; } = "parentId";

    // ─── pipeline ──────────────────────────────────────────────────────
    public override async Task ProcessAsync(TagHelperContext ctx, TagHelperOutput output)
    {
        await base.ProcessAsync(ctx, output);                 // let <option> list render

        // Marker attribute; JS will look for this
        output.Attributes.SetAttribute("data-s2-select", "true");

        // Guarantee an id  (needed for dependency toggle in JS)
        if (!output.Attributes.TryGetAttribute("id", out var idAttr) ||
            string.IsNullOrWhiteSpace(idAttr.Value?.ToString()))
        {
            var newId = "s2_" + Guid.NewGuid().ToString("N");
            output.Attributes.SetAttribute("id", newId);
        }

        // Plain data attributes (no JSON encoding headaches)
        output.Attributes.SetAttribute("data-s2-url",           DataUrl);
        output.Attributes.SetAttribute("data-s2-pagesize",      PageSize);
        if (MinInputLength   > 0) output.Attributes.SetAttribute("data-s2-minlen", MinInputLength);
        if (AllowClear)           output.Attributes.SetAttribute("data-s2-allowclear", "true");
        if (Multiple)             output.Attributes.SetAttribute("data-s2-multiple",   "true");
        if (Tags)                 output.Attributes.SetAttribute("data-s2-tags",       "true");
        if (!string.IsNullOrWhiteSpace(Placeholder))
                                   output.Attributes.SetAttribute("data-s2-placeholder", Placeholder);
        if (!string.IsNullOrWhiteSpace(Width))
                                   output.Attributes.SetAttribute("data-s2-width", Width);

        if (ExtraParams is not null)
            output.Attributes.SetAttribute("data-s2-extra",     global::System.Text.Json.JsonSerializer.Serialize(ExtraParams));

        if (!string.IsNullOrWhiteSpace(DependsOnSelector))
        {
            output.Attributes.SetAttribute("data-s2-dependson",  DependsOnSelector);
            output.Attributes.SetAttribute("data-s2-dependsparam", DependsParam);
        }
    }
}
