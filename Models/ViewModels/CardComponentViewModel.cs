using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace DotnetSkeletonApp.Models.ViewModels
{
    public class CardComponentViewModel
    {
        public string Title { get; set; } = default!;
        public IHtmlContent Body { get; set; } = HtmlString.Empty;
    }
}