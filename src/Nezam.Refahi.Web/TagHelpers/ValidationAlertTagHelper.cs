using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Nezam.Refahi.Web.TagHelpers;

  [HtmlTargetElement("validation-alert")]
  public class ValidationAlertTagHelper : TagHelper
  {
      [ViewContext]
      [HtmlAttributeNotBound]
      public required ViewContext ViewContext { get; set; }

      public override void Process(TagHelperContext context, TagHelperOutput output)
      {
          if (!ViewContext.ModelState.IsValid)
          {
              var errors = ViewContext.ViewData.ModelState.Values
                  .SelectMany(v => v.Errors)
                  .Select(e => e.ErrorMessage)
                  .ToList();

              var alertTagHelper = new AlertTagHelper
              {
                  Type = "danger",
                  Message = string.Join("<br/>", errors),
                  Icon = "ri-error-warning-line",
                  Dismissible = true
              };

              var alertContext = new TagHelperContext(
                  new TagHelperAttributeList(),
                  new Dictionary<object, object>(),
                  Guid.NewGuid().ToString("N"));

              var alertOutput = new TagHelperOutput(
                  "alert",
                  new TagHelperAttributeList(),
                  (useCachedResult, encoder) =>
                  {
                      var tagHelperContent = new DefaultTagHelperContent();
                      tagHelperContent.SetHtmlContent(string.Empty);
                      return Task.FromResult<TagHelperContent>(tagHelperContent);
                  });

              alertTagHelper.Process(alertContext, alertOutput);

              output.TagName = alertOutput.TagName;
              foreach (var attribute in alertOutput.Attributes)
              {
                  output.Attributes.SetAttribute(attribute.Name, attribute.Value);
              }
              output.Content.SetHtmlContent(alertOutput.Content.GetContent());
          }
          else
          {
              output.SuppressOutput();
          }
      }
  }
