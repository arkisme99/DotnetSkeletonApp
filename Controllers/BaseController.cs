using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DotnetSkeletonApp.Models.UserModels;
using DotnetSkeletonApp.Models.ViewModels;
using DotnetSkeletonApp.Services;
using Microsoft.AspNetCore.Identity;
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
        ActionExecutionDelegate next
        )
        {
            var userService = context.HttpContext.RequestServices.GetService(typeof(UserService)) as UserService;

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

            //Get Current User
            // string? currentUserName = User.Identity?.Name;
            // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            // var userNow = await _userManager.FindByIdAsync(userId);

            // var profileImage = userNow.Photo == null ? "/sources/img/demo/avatars/avatar-admin.png" : $"/uploads/avatar/{userNow.Photo}";


            ViewData["nameApp"] = nameApp;
            ViewData["logoPath"] = logoPath;
            ViewData["planApp"] = planApp;
            ViewData["versiApp"] = versiApp;


            // ViewData["profileImage"] = profileImage;

            var currentController = Request.RouteValues["controller"]?.ToString() ?? "";
            ViewData["currentController"] = currentController;
            ViewData["Title"] = currentController;

            base.OnActionExecuting(context);
        }


    }
}