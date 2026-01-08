using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetSkeletonApp.Models.ViewModels
{
    public record BreadcrumbsViewModel(
        string Label,
        bool Active = false,
        string? AspController = null,
        string? Action = null
    );
}