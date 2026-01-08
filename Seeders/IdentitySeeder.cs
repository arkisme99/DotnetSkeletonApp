using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Data;
using DotnetSkeletonApp.Models.UserModels;
using Microsoft.AspNetCore.Identity;

namespace DotnetSkeletonApp.Seeders
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // 1. Seed Roles
            var roles = new List<ApplicationRole>
            {
                new() { Name = "Administrator", NormalizedName = "ADMININSTRATOR", Description = "Administrator role" }
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role.Name!))
                {
                    await roleManager.CreateAsync(role);
                }
            }

            // 2. Seed Permissions
            var permissions = new List<Permission>
            {
                new() { Name = "View_User" },
                new() { Name = "Create_User" },
                new() { Name = "Edit_User" },
                new() { Name = "Delete_User" },
                new() { Name = "MultiDelete_User" },
                new() { Name = "View_Role" },
                new() { Name = "Create_Role" },
                new() { Name = "Edit_Role" },
                new() { Name = "Delete_Role" },
                new() { Name = "MultiDelete_Role" }
            };

            foreach (var permission in permissions)
            {
                if (!dbContext.Permissions.Any(p => p.Name == permission.Name))
                {
                    dbContext.Permissions.Add(permission);
                }
            }
            await dbContext.SaveChangesAsync();

            // 3. Assign Permissions to Administrator role
            var adminRole = await roleManager.FindByNameAsync("Administrator");
            if (adminRole != null)
            {
                var allPermissions = dbContext.Permissions.ToList();

                foreach (var perm in allPermissions)
                {
                    bool alreadyAssigned = dbContext.RolePermissions
                        .Any(rp => rp.RoleId == adminRole.Id && rp.PermissionId == perm.Id);

                    if (!alreadyAssigned)
                    {
                        dbContext.RolePermissions.Add(new ApplicationRolePermission
                        {
                            RoleId = adminRole.Id,
                            PermissionId = perm.Id
                        });
                    }
                }
                await dbContext.SaveChangesAsync();
            }

            // 4. Seed Default Admin User
            string adminEmail = "admin@test.com";
            string fullName = "Admin Aplikasi";
            string adminPassword = "admin12345"; // hashed otomatis

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var user = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = fullName,
                };

                var result = await userManager.CreateAsync(user, adminPassword);

                if (result.Succeeded && adminRole != null)
                {
                    await userManager.AddToRoleAsync(user, adminRole.Name!);
                }
            }
        }
    }
}