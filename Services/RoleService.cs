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
    }
}