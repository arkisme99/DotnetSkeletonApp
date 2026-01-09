using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DotnetSkeletonApp.TagHelpers
{
    public class CrudTableTagHelper : TagHelper
    {
        [HtmlTargetElement("table-layout")]
        public class TableLayoutTagHelper : TagHelper
        {
            public string TableId { get; set; } = "tableExample";

            public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
            {
                output.TagName = "div";
                output.Attributes.SetAttribute("class", "table-responsive");

                var content = await output.GetChildContentAsync();

                var template = $@"
                <table id='{TableId}' class='table table-bordered table-hover'>
                    {content.GetContent()}
                </table>";

                output.Content.SetHtmlContent(template);
            }
        }
    }
}