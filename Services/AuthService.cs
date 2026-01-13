using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DotnetSkeletonApp.Data;
using DotnetSkeletonApp.Models.UserModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DotnetSkeletonApp.Services
{
    public class AuthService(
        UserManager<ApplicationUser> _userManager,
        SignInManager<ApplicationUser> _signInManager,
        ApplicationDbContext _dbcontext,
        ILogger<AuthService> _logger,
        ActivityLogService _activityLogService,
        IHttpContextAccessor _httpContextAccessor
    )
    {
        public async Task<bool> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) return false;

            // Console.WriteLine("User found: {user}, Password Type : {password}", user.Email, password);
            // 🔹 cek password
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
            if (!result.Succeeded) return false;

            // 🔹 ambil role user
            var roles = await _userManager.GetRolesAsync(user);

            // 🔹 ambil permission dari role
            var permissions = await _dbcontext.RolePermissions
                .Where(rp => roles.Contains(rp.Role!.Name!))
                .Select(rp => rp.Permission!.Name)
                .ToListAsync();

            // 🔹 build claims
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.UserName ?? user.Email ?? email),
                new(ClaimTypes.Name, user.FullName ?? "Nama Lengkap"),
                new("ProfilePhoto", user.Photo is null ? "/sources/img/demo/avatars/avatar-admin.png" : $"/uploads/avatar/{user.Photo}" ?? "/sources/img/demo/avatars/avatar-admin.png")
            };

            // role → claim
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            // permission → claim
            claims.AddRange(permissions.Select(p => new Claim("Permission", p)));

            // 🔹 buat identity baru
            var claimsIdentity = new ClaimsIdentity(claims, "Identity.Application");

            // 🔹 sign in ulang dengan claims
            await _signInManager.SignOutAsync(); // clear dulu biar gak duplikat
            await _signInManager.Context.SignInAsync(
                "Identity.Application",
                new ClaimsPrincipal(claimsIdentity)
            );

            _logger.LogInformation("Proses Login {email}, {roles}, {permissions}", email, roles, permissions);
            await _activityLogService.LogChangeAsync(null, "Login", user.Id, null, null);

            return true;
        }

        public async Task LogoutAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            await _activityLogService.LogChangeAsync(null, "Logout", userId, null, null);
            await _signInManager.SignOutAsync();
        }
    }
}