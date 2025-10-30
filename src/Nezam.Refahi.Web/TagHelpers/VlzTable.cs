using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Nezam.Refahi.Web.TagHelpers;

// ───────────────────────────────────────── V L Z   B A S E   T A B L E ───────────────────────────────────
public abstract class VlzBaseTableTagHelper : TagHelper
{
    // Table configuration properties
    [HtmlAttributeName("ajax-url")]        public string? AjaxUrl       { get; set; }
    [HtmlAttributeName("method")]          public string  Method        { get; set; } = "get";
    protected string NormalizedMethod => Method?.ToUpper() == "GET" || Method?.ToUpper() == "POST" ? Method.ToUpper() : "GET";
    [HtmlAttributeName("filter-form-id")]  public string? FilterFormId  { get; set; }
    [HtmlAttributeName("show-filter-row")] public bool    ShowFilterRow { get; set; }
    [HtmlAttributeName("table-id")]        public string? TableId       { get; set; }
    [HtmlAttributeName("page-size")]       public int     PageSize      { get; set; } = 10;
    [HtmlAttributeName("enable-sorting")]  public bool    EnableSorting { get; set; } = true;
    [HtmlAttributeName("enable-pagination")] public bool EnablePagination { get; set; } = true;
    [HtmlAttributeName("enable-column-selection")] public bool EnableColumnSelection { get; set; } = true;
    
    protected string GenerateTableId()
    {
        return TableId ?? $"vlz-table-{Guid.NewGuid():N}";
    }
}

// ───────────────────────────────────────── V L Z   C O L U M N ──────────────────────────────────────────
[HtmlTargetElement("vlz-table-column", ParentTag = "vlz-data-table")]
public sealed class VlzTableColumnTagHelper : TagHelper
{
    [HtmlAttributeName("name")]           public string  Name        { get; set; } = string.Empty;
    [HtmlAttributeName("title")]          public string  Title       { get; set; } = string.Empty;
    [HtmlAttributeName("class")]          public string? CssClass    { get; set; }
    [HtmlAttributeName("template")]       public string? TemplateId  { get; set; }
    [HtmlAttributeName("template-content")] public string? TemplateContent { get; set; }
    [HtmlAttributeName("filterable")]     public bool    Filterable  { get; set; }
    [HtmlAttributeName("filter-type")]    public string? FilterType  { get; set; }
    [HtmlAttributeName("sortable")]       public bool    Sortable    { get; set; } = true;
    [HtmlAttributeName("width")]          public string? Width       { get; set; }
    [HtmlAttributeName("visible")]        public bool    Visible     { get; set; } = true;
    [HtmlAttributeName("datatype")]       public string? DataType    { get; set; } = "text";
    [HtmlAttributeName("format")]         public string? Format      { get; set; }
    [HtmlAttributeName("searchable")]     public bool    Searchable  { get; set; } = true;
    [HtmlAttributeName("default-value")]  public string? DefaultValue { get; set; }
    [HtmlAttributeName("display-order")]  public int     DisplayOrder { get; set; } = 0;
    [HtmlAttributeName("tooltip")]        public string? Tooltip     { get; set; }
    [HtmlAttributeName("fixed")]          public bool    Fixed       { get; set; }

    [HtmlAttributeName("parent-id")]
    public string ParentId { get; set; } = string.Empty;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        // Get the child content if available (for inline templates)
        string? childContent = null;
        
        // First priority: Check if there's actual content inside the tag
        if (output.Content.IsModified || !output.Content.IsEmptyOrWhiteSpace)
        {
            childContent = (await output.GetChildContentAsync()).GetContent();
        }
        // Second priority: Check if template-content attribute is provided
        else if (context.AllAttributes.ContainsName("template-content"))
        {
            childContent = TemplateContent;
        }
        
        var column = new VlzTableColumn
        {
            Name           = Name,
            Title          = Title,
            CssClass       = CssClass,
            TemplateId     = TemplateId,
            TemplateContent = childContent,
            Filterable     = Filterable,
            FilterType     = FilterType,
            Sortable       = Sortable,
            Width          = Width,
            Visible        = Visible,
            DataType       = DataType,
            Format         = Format,
            Searchable     = Searchable,
            DefaultValue   = DefaultValue,
            Order          = DisplayOrder,
            Tooltip        = Tooltip,
            Fixed          = Fixed
        };

        // Switch to an <input /> with self-closing mode
        output.TagName  = "input";
        output.TagMode  = TagMode.SelfClosing;
        output.Attributes.SetAttribute("type", "hidden");
        output.Attributes.SetAttribute("class", "vlz-column-config");
        output.Attributes.SetAttribute("data-parent-id", ParentId);
        output.Attributes.SetAttribute("data-column-name", Name);
        
        // Store template content in a data attribute if available
        if (!string.IsNullOrEmpty(childContent))
        {
            // Store the template content directly
            output.Attributes.SetAttribute("data-template-content", childContent);
        }

        // Reflect over the VlzTableColumn props and add each as a data-* attribute
        foreach (var prop in typeof(VlzTableColumn).GetProperties())
        {
            var rawValue = prop.GetValue(column, null);
            if (rawValue == null) 
                continue;

            string stringValue;

            // Booleans and numbers can use ToString(); strings we trim
            switch (rawValue)
            {
                case bool b:
                    stringValue = b.ToString().ToLowerInvariant(); 
                    break;
                case int i:
                    stringValue = i.ToString();
                    break;
                default:
                    stringValue = rawValue.ToString()!;
                    if (string.IsNullOrWhiteSpace(stringValue))
                        continue;
                    break;
            }

            // convert PascalCase to kebab-case for data- attribute name
            
            var kebab = global::System.Text.RegularExpressions.Regex.Replace(prop.Name, "([a-z])([A-Z])", "$1-$2")
                            .ToLowerInvariant();

            output.Attributes.SetAttribute($"data-{kebab}", stringValue);
        }
    }
}


// ───────────────────────────────────────── V L Z   D A T A   T A B L E ───────────────────────────────────
[HtmlTargetElement("vlz-data-table")]
public sealed class VlzDataTableTagHelper : VlzBaseTableTagHelper
{
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var childHtml = (await output.GetChildContentAsync()).GetContent();
        var tableId   = GenerateTableId();

        output.TagName = "div";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.SetAttribute("class", "table-responsive");
        var html = $@"
{childHtml}
<table
    id=""{tableId}""
    class=""table table-hover table-bordered table-nowrap align-middle mb-0 vlz-data-table""
    data-ajax-url=""{AjaxUrl}""
    data-method=""{NormalizedMethod}""
    data-page-size=""{PageSize}""
    data-enable-sorting=""{EnableSorting.ToString().ToLowerInvariant()}""
    data-enable-pagination=""{EnablePagination.ToString().ToLowerInvariant()}""
    data-enable-column-selection=""{EnableColumnSelection.ToString().ToLowerInvariant()}""
    data-filter-form-id=""{FilterFormId}""
    data-show-filter-row=""{ShowFilterRow.ToString().ToLowerInvariant()}"">
  <thead></thead>
  <tbody>
    <tr><td colspan=""1"" class=""text-center py-4"">در حال بارگذاری…</td></tr>
  </tbody>
</table>";

        // Add initialization script
        html += $@"
<script>
  document.addEventListener('DOMContentLoaded', function() {{
    if (window.VlzTable) {{
      window.VlzTable.initTable(document.getElementById('{tableId}'));
    }} else {{
      console.error('VlzTable script not loaded. Make sure to include vlz-table.js in your page.');
    }}
  }});
</script>";

        output.Content.SetHtmlContent(html);
    }
}


// ───────────────────────────────────────── P O C O ─────────────────────────────────────────────────────
public sealed class VlzTableColumn
{
    public string  Name           { get; init; } = string.Empty;
    public string  Title          { get; init; } = string.Empty;
    public string? CssClass       { get; init; }
    public bool    Filterable     { get; init; }
    public string? FilterType     { get; init; }
    public string? TemplateId     { get; init; }
    public string? TemplateContent { get; init; }
    public bool    Sortable       { get; init; } = true;
    public string? Width      { get; init; }
    public bool    Visible    { get; init; } = true;
    public string? DataType   { get; init; } = "text";
    public string? Format     { get; init; }
    public bool    Searchable { get; init; } = true;
    public string? DefaultValue { get; init; }
    public int     Order      { get; init; } = 0; // Column display order
    public string? Tooltip    { get; init; }
    public bool    Fixed      { get; init; }
    
    [JsonIgnore]
    public Dictionary<string, string> CustomAttributes { get; init; } = new();
}
