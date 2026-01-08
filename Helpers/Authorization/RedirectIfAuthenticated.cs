using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DotnetSkeletonApp.Helpers.Authorization
{
    public class RedirectIfAuthenticated : IAsyncAuthorizationFilter
    {
        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                context.Result = new RedirectToActionResult(
                    "Index",
                    "Home",
                    null
                );
            }

            return Task.CompletedTask;
        }
    }
}