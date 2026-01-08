using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DotnetSkeletonApp.Controllers
{
    public class BaseController : Controller
    {
        protected void SetBreadcrumbs(params BreadcrumbsViewModel[] items)
        {
            ViewData["Breadcrumbs"] = items.ToList();
        }

        public override async void OnActionExecuting(ActionExecutingContext context)
        {
            var nameApp = "Skeleton Dotnet App";
            var logoPath = "/sources/img/favicon/fav.png";
            var planApp = "Premium";
            var versiApp = "1.0.1";
            var profileImage = "/sources/img/demo/avatars/avatar-admin.png";

            ViewData["nameApp"] = nameApp;
            ViewData["logoPath"] = logoPath;
            ViewData["planApp"] = planApp;
            ViewData["versiApp"] = versiApp;
            ViewData["profileImage"] = profileImage;

            base.OnActionExecuting(context);
        }

    }
}