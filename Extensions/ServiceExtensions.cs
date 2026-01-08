using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Services;
using DotnetSkeletonApp.Services.Recaptcha;

namespace DotnetSkeletonApp.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<RecaptchaServices>();
            // services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<EmailService>();
            // services.AddScoped<IRoleService, RoleService>();
            // services.AddScoped<IUserService, UserService>();
            // services.AddScoped<IActivityLogService, ActivityLogService>();
            // services.AddScoped<INotificationService, NotificationService>();

            return services;
        }

    }
}