using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DotnetSkeletonApp.TagHelpers
{
    [HtmlTargetElement("card-layout")]
    public class CardLayoutTagHelper : TagHelper
    {
        public string Title { get; set; } = "Card Layout";
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "panel");

            // Ambil konten di dalam tag <card-layout>...</card-layout>
            var content = await output.GetChildContentAsync();

            var template = $@"
                <div class='panel-hdr bg-primary'>
                    <h2 class='text-white'>{Title}</h2>
                    <div class='panel-toolbar'>
                        <div class='btn-group' id='js-demo-nesting' role='group' aria-label='Button group with nested dropdown'>
                            <button type='button' class='btn btn-secondary'>1</button>
                            <button type='button' class='btn btn-secondary'>2</button>
                            <div class='btn-group' role='group'>
                                <button type='button' class='btn btn-secondary dropdown-toggle' data-toggle='dropdown'>Dropdown</button>
                                <div class='dropdown-menu'>
                                    <a class='dropdown-item' href='javascript:void(0)'>Dropdown link</a>
                                    <a class='dropdown-item' href='javascript:void(0)'>Dropdown link</a>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class='panel-container'>
                    <div class='panel-content'>
                        {content.GetContent()}
                    </div>
                </div>";

            output.Content.SetHtmlContent(template);
        }
    }
}