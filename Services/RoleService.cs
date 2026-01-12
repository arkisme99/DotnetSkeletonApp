using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Data;
using DotnetSkeletonApp.Models.UserModels;
using Microsoft.EntityFrameworkCore;

namespace DotnetSkeletonApp.Services
{
    public class RoleService(
        ApplicationDbContext _context,
        PermissionService permissionService
    ) : BaseCrudService<ApplicationRole, string>(
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
                { "Permissions", groupedPermissions }
            };
        }

        public override async Task<Dictionary<string, object>> EditData(string Id)
        {

            var groupedPermissions = await _permissionService.GetPermissionsAsync();
            var currentRolePermissions = await _permissionService.GetRoleWithPermissionsAsync(Id.ToString());

            return new Dictionary<string, object>
            {
                { "Permissions", groupedPermissions },
                { "CurrentRolePermissions", currentRolePermissions }
            };
        }

        protected override async Task<ApplicationRole> AfterCreateAsync(ApplicationRole model, IFormCollection RawFormData)
        {

            var selectedPermissionNames = RawFormData!["choosePermissions[]"].ToList();

            if (selectedPermissionNames != null && selectedPermissionNames.Count != 0)
            {

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

        protected override async Task<ApplicationRole> AfterUpdateAsync(ApplicationRole model, IFormCollection RawFormData)
        {
            var selectedPermissionNames = RawFormData!["choosePermissions[]"].ToList();

            // Ambil permission lama
            var oldPermissionIds = await _context.RolePermissions
                .Where(rp => rp.RoleId == model.Id)
                .Select(rp => rp.PermissionId)
                .ToListAsync();

            // Ambil permissionId dari selectedPermissionNames
            var newPermissionIds = await _context.Permissions
                .Where(p => selectedPermissionNames.Contains(p.Name))
                .Select(p => p.Id)
                .ToListAsync();

            // Hitung diff
            var toAdd = newPermissionIds.Except(oldPermissionIds).ToList();
            var toRemove = oldPermissionIds.Except(newPermissionIds).ToList();

            // Remove yang tidak dipakai lagi
            if (toRemove.Count > 0)
            {
                var removeEntities = _context.RolePermissions
                    .Where(rp => rp.RoleId == model.Id && toRemove.Contains(rp.PermissionId));
                _context.RolePermissions.RemoveRange(removeEntities);
            }

            // Tambahkan yang baru
            if (toAdd.Count > 0)
            {
                var addEntities = toAdd.Select(pid => new ApplicationRolePermission
                {
                    RoleId = model.Id,
                    PermissionId = pid
                });
                await _context.RolePermissions.AddRangeAsync(addEntities);
            }

            await _context.SaveChangesAsync();

            return model;
        }
    }
}