using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Data;
using DotnetSkeletonApp.Models.UserModels;
using Microsoft.AspNetCore.Identity;

namespace DotnetSkeletonApp.Extensions
{
    public static class IdentityExtensions
    {
        public static IServiceCollection AddIdentityWithCookie(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/auth/login";
                options.LogoutPath = "/auth/logout";
                options.AccessDeniedPath = "/home/index";

                options.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents
                {
                    OnRedirectToLogin = ctx =>
                    {
                        /* var redirect = $"{ctx.RedirectUri}{(ctx.RedirectUri.Contains('?') ? "&" : "?")}message=Harus+login+dulu+bro!";
                        ctx.Response.Redirect(redirect); */
                        ctx.Response.Cookies.Append(
                        "notify_error",
                        "Harus login dulu bro!",
                        new CookieOptions
                        {
                            HttpOnly = true,
                            IsEssential = true
                        });

                        ctx.Response.Redirect(ctx.RedirectUri);
                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = ctx =>
                    {
                        /* var redirect = $"{ctx.RedirectUri}{(ctx.RedirectUri.Contains('?') ? "&" : "?")}message=Anda+Gak+punya+izin+bro!";
                        ctx.Response.Redirect(redirect); */

                        ctx.Response.Cookies.Append(
                        "notify_error",
                        "Harus login dulu bro!",
                        new CookieOptions
                        {
                            HttpOnly = true,
                            IsEssential = true
                        });

                        ctx.Response.Redirect(ctx.RedirectUri);
                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }

    }
}