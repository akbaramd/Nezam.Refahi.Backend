using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Nezam.Refahi.Web.TagHelpers;

[HtmlTargetElement("card-link", Attributes = "href")]
public class CardLinkTagHelper : TagHelper
{
    private readonly IHtmlGenerator _generator;
    private readonly HtmlEncoder _htmlEncoder;

    public CardLinkTagHelper(IHtmlGenerator generator, HtmlEncoder htmlEncoder)
    {
        _generator = generator;
        _htmlEncoder = htmlEncoder;
    }

    [HtmlAttributeName("href")]
    public string Href { get; set; } = string.Empty;

    [HtmlAttributeName("icon")]
    public string Icon { get; set; } = string.Empty;

    [HtmlAttributeName("title")]
    public string Title { get; set; } = string.Empty;

    [HtmlAttributeName("description")]
    public string Description { get; set; } = string.Empty;

    [HtmlAttributeName("icon-color")]
    public string IconColor { get; set; } = "primary";

    [HtmlAttributeName("icon-bg-color")]
    public string IconBgColor { get; set; } = "primary-subtle";

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "a";
        output.TagMode = TagMode.StartTagAndEndTag;

        // Set the href attribute
        output.Attributes.SetAttribute("href", Href);

        // Get the content
        var content = await output.GetChildContentAsync();

        // Create the card structure
        var cardHtml = $@"
            <div class='card border shadow-sm h-100'>
                <div class='card-body p-0'>
                    <div class='d-flex flex-column align-items-center justify-content-center p-4 text-center'>
                        <div class='mb-3'>
                            <div class='avatar-sm bg-{IconBgColor} text-{IconColor} rounded'>
                                <span class='avatar-title'>
                                    <i class='{Icon} fs-20'></i>
                                </span>
                            </div>
                        </div>
                        <h5 class='mb-1'>{Title}</h5>
                        <p class='text-muted mb-0 fs-13'>{Description}</p>
                    </div>
                </div>
            </div>";

        output.Content.SetHtmlContent(cardHtml);
    }
}