using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetSkeletonApp.Models.ViewModels
{
    /* public record BreadcrumbsViewModel(
        string Label,
        bool Active = false,
        string? AspController = null,
        string? Action = null
    ); */
    public class BreadcrumbsViewModel(string label, bool active = false, string? aspController = null, string? action = null)
    {
        public string Label { get; set; } = label;
        public bool Active { get; set; } = active;
        public string? AspController { get; set; } = aspController;
        public string? Action { get; set; } = action;
    }
}