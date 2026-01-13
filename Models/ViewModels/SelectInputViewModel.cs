using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetSkeletonApp.Models.ViewModels
{
    public class SelectInputViewModel
    {
        public string? Label { get; set; }
        public string Name { get; set; } = default!;
        public string Id { get; set; } = default!;

        public string? AddNewClass { get; set; }
        public string? Attributes { get; set; }
    }
}