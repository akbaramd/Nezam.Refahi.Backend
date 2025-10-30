using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nezam.New.EES.Utilities;

  public static class HtmlHelperExtensions
  {
      /// <summary>
      /// Adds the UserSelectedInput component scripts and styles to the page
      /// </summary>
      public static IHtmlContent UserSelectedInputAssets(this IHtmlHelper htmlHelper)
      {
          var cssLink = new TagBuilder("link");
          cssLink.Attributes.Add("href", "~/css/user-selected-input.css");
          cssLink.Attributes.Add("rel", "stylesheet");
          cssLink.Attributes.Add("type", "text/css");
          
          var jsScript = new TagBuilder("script");
          jsScript.Attributes.Add("src", "~/js/modal-select-input.js");
          
          return new HtmlContentBuilder()
              .AppendHtml(cssLink)
              .AppendHtml("\n")
              .AppendHtml(jsScript);
      }
  }
