using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Data;
using DotnetSkeletonApp.Models.UserModels;
using Microsoft.AspNetCore.Identity;

namespace DotnetSkeletonApp.Seeders
{
    public static class SetupAppPermissionSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

            var permissions = new List<Permission>
            {
                new() { Name = "View_Setup" },
                new() { Name = "Edit_Setup" }
            };

            foreach (var permission in permissions)
            {
                if (!dbContext.Permissions.Any(p => p.Name == permission.Name))
                {
                    dbContext.Permissions.Add(permission);
                }
            }

            await dbContext.SaveChangesAsync();

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
        }
    }
}