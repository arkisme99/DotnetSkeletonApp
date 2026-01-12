using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetSkeletonApp.Models.ViewModels
{
    public class InputImageViewModel
    {
        public string? Label { get; set; }
        public string Name { get; set; } = default!;
        public string Id { get; set; } = default!;
        public string? AddNewClass { get; set; }

        // attribute bebas (pengganti Blade attribute bag)
        public string? Attributes { get; set; }
    }
}