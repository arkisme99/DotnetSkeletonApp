using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetSkeletonApp.Models.ViewModels;

public class InputGroupViewModel
{
    public string? Label { get; set; }
    public string Type { get; set; } = "text";
    public string Name { get; set; } = default!;
    public string Id { get; set; } = default!;
    public string? PlaceHolder { get; set; }
    public string? AddNewClass { get; set; }
    public string? Icon { get; set; }
    public string? DataTargetIcon { get; set; }
    public bool NoId { get; set; } = false;

    // attribute bebas (pengganti Blade attribute bag)
    public string? Attributes { get; set; }
}
