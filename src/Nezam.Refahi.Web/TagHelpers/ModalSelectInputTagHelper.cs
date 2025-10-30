using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Nezam.Refahi.Web.TagHelpers;

/// <summary>
/// Select2 + Modal integration component rendered with:
/// <code>
/// &lt;user-selected-input asp-for="RecipientIds"
///                       api-url="/api/recipients/lookup"
///                       extra-filters="letterTypeId=3&source=mobile" /&gt;
/// </code>
/// Features:
/// - Select2 input with server-side search
/// - Modal button for bulk selection
/// - Synchronized selections between Select2 and modal
/// </summary>
[HtmlTargetElement("modal-select", TagStructure = TagStructure.WithoutEndTag)]
public sealed class ModalSelectInputTagHelper : TagHelper
{
    private static readonly HtmlEncoder Enc = HtmlEncoder.Default;

    /* ---------- public attributes ---------------------------------- */
    [HtmlAttributeName("asp-for")]        public ModelExpression For { get; set; } = default!;
    [HtmlAttributeName("api-url")]        public string ApiUrl   { get; set; } = "/api/lookup";
    [HtmlAttributeName("text-field")]     public string TextFld  { get; set; } = "text";
    [HtmlAttributeName("selection-mode")] public string Mode     { get; set; } = "multiple"; // or single
    [HtmlAttributeName("placeholder")]    public string Placeholder { get; set; } = "انتخاب کاربر...";
    [HtmlAttributeName("button-text")]    public string BtnText  { get; set; } = "انتخاب از لیست";
    [HtmlAttributeName("modal-title")]    public string Title    { get; set; } = "انتخاب کاربران";
    [HtmlAttributeName("selected-json")]  public string SelJson  { get; set; } = "";

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
        output.Attributes.SetAttribute("class", "user-selected-input-container");

        string inputId = $"user_select_{For?.Name?.Replace('.', '_')}";
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
            <select class="form-control user-select2" 
                    id="{inputId}_select2" 
                    name="{For?.Name}"
                    data-placeholder="{Enc.Encode(Placeholder)}"
                    {(Mode == "multiple" ? "multiple" : "")}>
            </select>
            <button type="button" class="btn btn-outline-secondary user-modal-btn"
                    id="{inputId}_modal_btn"
                    data-bs-toggle="modal" data-bs-target="#{modalId}">
                <i class="ri-list-check"></i> {Enc.Encode(BtnText)}
            </button>
        </div>

        <div class="modal fade" id="{modalId}" tabindex="-1" aria-hidden="true"
             aria-labelledby="{modalId}Label">
          <div class="modal-dialog modal-lg modal-dialog-scrollable">
            <div class="modal-content">
              <div class="modal-header">
                <h5 class="modal-title" id="{modalId}Label">{Enc.Encode(Title)}</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
              </div>
              <div class="modal-body p-0" dir="rtl"></div>
              <div class="modal-footer">
                <button type="button" class="btn btn-primary user-modal-commit"
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
        if (!string.IsNullOrWhiteSpace(ExtraFilters))
            output.Attributes.SetAttribute("data-extra-filters", ExtraFilters);

        if (!string.IsNullOrWhiteSpace(SelJson))
            output.Attributes.SetAttribute("data-selected-items", SelJson);
        else if (!string.IsNullOrWhiteSpace(initialVal))
            output.Attributes.SetAttribute("data-selected-items", initialVal);
    }
}
