using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetSkeletonApp.Models.ViewModels
{
    public class InputCheckboxViewModel
    {
        public string Label { get; set; } = default!;
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Color { get; set; } = "primary";
        public string? AddNewClass { get; set; }
        public string? Attributes { get; set; }
    }
}