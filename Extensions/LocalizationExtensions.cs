using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using FluentValidation.AspNetCore;
using DotnetSkeletonApp.Validator;
using FluentValidation;

namespace DotnetSkeletonApp.Extensions
{
    public static class LocalizationExtensions
    {
        public static IServiceCollection AddLocalizationSupport(this IServiceCollection services)
        {
            services.AddLocalization(opt => opt.ResourcesPath = "Resources");

            services.AddControllersWithViews()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var cultures = new[] { new CultureInfo("en"), new CultureInfo("id") };
                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
                options.RequestCultureProviders = [new CookieRequestCultureProvider()];
            });

            return services;
        }
    }
}