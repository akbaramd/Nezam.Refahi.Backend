using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Nezam.Refahi.Web.TagHelpers;

[HtmlTargetElement("vlz-card")]
public class VlzCardTagHelper : TagHelper
{
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
    output.TagName = "div";
    output.TagMode = TagMode.StartTagAndEndTag;
    output.Attributes.SetAttribute("class", "card");
    output.Content.AppendHtml(output.GetChildContentAsync().Result.GetContent());
  }
}

[HtmlTargetElement("vlz-card-header", ParentTag = "vlz-card")]
public class VlzCardHeaderTagHelper : TagHelper
{
  [HtmlAttributeName("class")]
  public string? CssClass { get; set; }

  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
    output.TagName = "div";
    output.TagMode = TagMode.StartTagAndEndTag;

    var classes = "card-header border-bottom-dashed";
    if (!string.IsNullOrWhiteSpace(CssClass))
    {
      classes += " " + CssClass;
    }

    output.Attributes.SetAttribute("class", classes);
    output.Content.SetHtmlContent(output.GetChildContentAsync().Result.GetContent());
  }
}




[HtmlTargetElement("vlz-card-header-expandable", ParentTag = "vlz-card-header")]
public class VlzCardHeaderExpandableTagHelper : TagHelper
{
  [HtmlAttributeName("id")]
  public string? Id { get; set; }

  [HtmlAttributeName("class")]
  public string? CssClass { get; set; }

  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
    output.TagName = "div";
    output.TagMode = TagMode.StartTagAndEndTag;

    if (!string.IsNullOrWhiteSpace(Id))
    {
      output.Attributes.SetAttribute("id", Id);
    }

    var classes = "collapse mt-3";
    if (!string.IsNullOrWhiteSpace(CssClass))
    {
      classes += " " + CssClass;
    }

    output.Attributes.SetAttribute("class", classes);
    output.Content.SetHtmlContent(output.GetChildContentAsync().Result.GetContent());
  }
}


[HtmlTargetElement("vlz-card-body", ParentTag = "vlz-card")]
public class VlzCardBodyTagHelper : TagHelper
{
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
    output.TagName = "div";
    output.Attributes.SetAttribute("class", "card-body");
    output.Content.SetHtmlContent(output.GetChildContentAsync().Result.GetContent());
  }
}

