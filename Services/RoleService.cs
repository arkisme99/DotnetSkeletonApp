using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Data;
using DotnetSkeletonApp.Models.UserModels;

namespace DotnetSkeletonApp.Services
{
    public class RoleService(
        ApplicationDbContext _context,
        PermissionService permissionService
    ) : BaseCrudService<ApplicationRole>(
        _context
    )
    {
        public readonly PermissionService _permissionService = permissionService;

        protected IFormCollection? RawFormData { get; private set; }

        public override async Task<Dictionary<string, object>> CreateData()
        {

            var groupedPermissions = await _permissionService.GetPermissionsAsync();

            return new Dictionary<string, object>
            {
                { "Permissions", groupedPermissions },
                // Anda bisa tambah data lain di sini, misal:
                // { "Categories", categoriesList }
            };
        }

        protected override async Task<ApplicationRole> AfterCreateAsync(ApplicationRole model, IFormCollection RawFormData)
        {
            // Console.WriteLine("Masuk AfterCreateAsync");
            // proses permission di sini
            var selectedPermissionNames = RawFormData!["choosePermissions[]"].ToList();
            // Console.WriteLine("Masuk AfterCreateAsync selectedPermissionNames : -> " + selectedPermissionNames.Count);
            if (selectedPermissionNames != null && selectedPermissionNames.Count != 0)
            {
                // Console.WriteLine("Selected Permissions: " + string.Join(", ", selectedPermissionNames));
                var permissions = _context.Permissions
                    .Where(p => selectedPermissionNames.Contains(p.Name))
                    .ToList();

                foreach (var perm in permissions)
                {
                    bool alreadyAssigned = _context.RolePermissions
                        .Any(rp => rp.RoleId == model.Id && rp.PermissionId == perm.Id);

                    if (!alreadyAssigned)
                    {
                        _context.RolePermissions.Add(new ApplicationRolePermission
                        {
                            RoleId = model.Id,
                            PermissionId = perm.Id
                        });
                    }
                }

                await _context.SaveChangesAsync();
            }

            return model;
        }
    }
}