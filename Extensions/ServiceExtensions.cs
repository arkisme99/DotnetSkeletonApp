using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Helpers.Authorization;
using DotnetSkeletonApp.Services;
using DotnetSkeletonApp.Services.Recaptcha;

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
            // services.AddScoped<IRoleService, RoleService>();
            // services.AddScoped<IActivityLogService, ActivityLogService>();

            return services;
        }

    }
}