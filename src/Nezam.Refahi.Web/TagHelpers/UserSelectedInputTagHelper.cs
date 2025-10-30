using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Nezam.Refahi.Web.TagHelpers;

/// <summary>
/// Re-usable "search & pick" component rendered with:
/// <code>
/// &lt;modal-select asp-for="RecipientIds"
///                api-url="/api/recipients/lookup"
///                extra-filters="letterTypeId=3&source=mobile" /&gt;
/// </code>
/// The <c>extra-filters</c> string is appended verbatim to every API call.
/// </summary>
[HtmlTargetElement("modal-select", TagStructure = TagStructure.WithoutEndTag)]
public sealed class ModalSelectTagHelper : TagHelper
{
    private static readonly HtmlEncoder Enc = HtmlEncoder.Default;

    /* ---------- public attributes ---------------------------------- */
    [HtmlAttributeName("asp-for")]        public ModelExpression For { get; set; } = default!;
    [HtmlAttributeName("api-url")]        public string ApiUrl   { get; set; } = "/api/lookup";
    [HtmlAttributeName("text")]           public string TextFld  { get; set; } = "text";
    [HtmlAttributeName("selection-mode")] public string Mode     { get; set; } = "multiple"; // or single
    [HtmlAttributeName("placeholder")]    public string Placeholder { get; set; } = "انتخاب...";
    [HtmlAttributeName("button-text")]    public string BtnText  { get; set; } = "انتخاب";
    [HtmlAttributeName("modal-title")]    public string Title    { get; set; } = "انتخاب";
    [HtmlAttributeName("selected-json")]  public string SelJson  { get; set; } = "";
    [HtmlAttributeName("page-size")]      public int PageSize    { get; set; } = 10;
    [HtmlAttributeName("show-selected-first")] public bool ShowSelectedFirst { get; set; } = true;
    [HtmlAttributeName("allow-deselect")] public bool AllowDeselect { get; set; } = true;
    [HtmlAttributeName("min-search-length")] public int MinSearchLength { get; set; } = 2;

    /// <summary>
    /// Static query-string fragment to append to every request,
    /// e.g. <c>"letterTypeId=3&amp;source=mobile"</c>.
    /// </summary>
    [HtmlAttributeName("extra-filters")]  public string ExtraFilters { get; set; } = "";

    /* ---------- render --------------------------------------------- */
    public override void Process(TagHelperContext ctx, TagHelperOutput output)
    {
        output.TagName = "div";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.SetAttribute("class", "modal-select-container");

        string inputId = $"modal_select_{For?.Name?.Replace('.', '_')}";
        string modalId = $"{inputId}_modal";

        /* -------- pre-selected items (edit forms) -------------------- */
        string initialVal = For?.Model switch
        {
            null => "",
            IEnumerable<object> coll when For.Model is not string => JsonSerializer.Serialize(coll),
            _ => For!.Model!.ToString()!
        };

        /* -------- main HTML markup ----------------------------------- */
        var html = $"""
        <div class="input-group">
            <input type="text" class="form-control modal-select-display"
                   id="{inputId}_display" placeholder="{Enc.Encode(Placeholder)}" readonly>
            <span id="{inputId}_holder"></span>
            <button type="button" class="btn btn-outline-secondary modal-select-btn"
                    id="{inputId}_button"
                    data-bs-toggle="modal" data-bs-target="#{modalId}">
                <i class="ri-search-line"></i> {Enc.Encode(BtnText)}
            </button>
        </div>

        <div class="modal fade" id="{modalId}" tabindex="-1" aria-hidden="true"
             aria-labelledby="{modalId}Label">
          <div class="modal-dialog modal-lg modal-dialog-scrollable">
            <div class="modal-content">
              <div class="modal-header">
                <h5 class="modal-title" id="{modalId}Label">{Enc.Encode(Title)}</h5>
                <button type="button" class="btn btn-outline-primary btn-sm show-selected-btn" 
                        data-input-id="{inputId}">
                    <i class="ri-checkbox-multiple-line"></i> نمایش انتخاب شده‌ها
                </button>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
              </div>
              <div class="modal-body p-0" dir="rtl">
                <div class="search-section p-3 border-bottom">
                  <div class="input-group">
                    <input type="search" class="form-control search-box" placeholder="جستجو...">
                    <button class="btn btn-primary search-btn" type="button"><i class="ri-search-line"></i></button>
                  </div>
                </div>
                <div class="table-responsive">
                  <table class="table table-hover mb-0">
                    <thead><tr><th style="width:50px;"></th><th>عنوان</th><th>توضیحات</th></tr></thead>
                    <tbody class="tbl-body"></tbody>
                  </table>
                </div>
                <div class="d-flex justify-content-between align-items-center p-2">
                  <small class="tbl-info text-muted"></small>
                  <nav><ul class="pagination pager mb-0"></ul></nav>
                </div>
              </div>
              <div class="modal-footer">
                <button type="button" class="btn btn-primary modal-select-commit"
                        data-input-id="{inputId}">تایید</button>
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">بستن</button>
              </div>
            </div>
          </div>
        </div>
        """;

        output.Content.SetHtmlContent(html);

        /* -------- data-* attributes consumed by JS driver ------------ */
        output.Attributes.SetAttribute("data-input-id",         inputId);
        output.Attributes.SetAttribute("data-api-url",          ApiUrl);
        output.Attributes.SetAttribute("data-text-field",       TextFld);
        output.Attributes.SetAttribute("data-selection-mode",   Mode);
        output.Attributes.SetAttribute("data-field-name", For?.Name);
        output.Attributes.SetAttribute("data-page-size",        PageSize);
        output.Attributes.SetAttribute("data-show-selected-first", ShowSelectedFirst.ToString().ToLower());
        output.Attributes.SetAttribute("data-allow-deselect",   AllowDeselect.ToString().ToLower());
        output.Attributes.SetAttribute("data-min-search-length", MinSearchLength);
        
        if (!string.IsNullOrWhiteSpace(ExtraFilters))
            output.Attributes.SetAttribute("data-extra-filters", ExtraFilters);

        if (!string.IsNullOrWhiteSpace(SelJson))
            output.Attributes.SetAttribute("data-selected-items", SelJson);
        else if (!string.IsNullOrWhiteSpace(initialVal))
            output.Attributes.SetAttribute("data-selected-items", initialVal);
    }
}
