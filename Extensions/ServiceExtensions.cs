using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Helpers.Authorization;
using DotnetSkeletonApp.Models.ViewModels;
using DotnetSkeletonApp.Services;
using DotnetSkeletonApp.Services.Recaptcha;
using DotnetSkeletonApp.Validator;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace DotnetSkeletonApp.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<EmailService>();
            services.AddScoped<RecaptchaServices>();
            services.AddScoped<ActivityLogService>();
            services.AddScoped<AuthService>();
            services.AddScoped<RedirectIfAuthenticated>();
            services.AddScoped<NotificationService>();
            services.AddScoped<UserService>();
            services.AddScoped<RoleService>();
            services.AddScoped<PermissionService>();
            // services.AddScoped<IActivityLogService, ActivityLogService>();
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            // 3. Daftarkan Validator Anda
            // services.AddValidatorsFromAssemblyContaining<UserValidator>();
            services.AddScoped<IValidator<UserViewModel>, UserValidator>();

            return services;
        }

    }
}