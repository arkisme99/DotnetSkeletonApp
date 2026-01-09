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

        public override async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
        {
            await next();

            var request = context.HttpContext.Request;
            var response = context.HttpContext.Response;

            // 1️⃣ Dari TempData (CRUD)
            if (TempData.TryGetValue("Notify.Message", out object? value))
            {
                ViewData["TypeMessage"] = TempData["Notify.Type"];
                ViewData["ValueMessage"] = value;
            }

            // 2️⃣ Dari Cookie (Auth / Middleware)
            if (request.Cookies.TryGetValue("notify_error", out var cookieMessage))
            {
                ViewData["TypeMessage"] = "error";
                ViewData["ValueMessage"] = cookieMessage;

                response.Cookies.Delete("notify_error");
            }

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

            var currentController = Request.RouteValues["controller"]?.ToString() ?? "";
            ViewData["currentController"] = currentController;
            ViewData["Title"] = currentController;

            base.OnActionExecuting(context);
        }


    }
}