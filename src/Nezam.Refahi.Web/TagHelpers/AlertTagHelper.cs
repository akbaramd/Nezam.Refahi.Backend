using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Nezam.Refahi.Web.TagHelpers;

  [HtmlTargetElement("alert")]
  public class AlertTagHelper : TagHelper
  {
      [HtmlAttributeName("type")]
      public string Type { get; set; } = "danger";

      [HtmlAttributeName("message")]
      public string Message { get; set; } = string.Empty;

      [HtmlAttributeName("dismissible")]
      public bool Dismissible { get; set; } = true;

      [HtmlAttributeName("icon")]
      public string Icon { get; set; } = "ri-error-warning-line";

      public override void Process(TagHelperContext context, TagHelperOutput output)
      {
          output.TagName = "div";
          output.Attributes.SetAttribute("class", $"alert alert-{Type} alert-dismissible alert-label-icon label-arrow fade show mb-3");
          output.Attributes.SetAttribute("role", "alert");
          output.Attributes.SetAttribute("tabindex", "-1");

          var content = $@"
                <i class='{Icon} label-icon'></i>
                <div>
                    <p class='mb-0'>{Message}</p>
                </div>";

          if (Dismissible)
          {
              content += "<button type='button' class='btn-close' data-bs-dismiss='alert' aria-label='Close'></button>";
          }

          output.Content.SetHtmlContent(content);
      }
  }
