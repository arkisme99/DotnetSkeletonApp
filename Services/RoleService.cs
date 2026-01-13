using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Data;
using DotnetSkeletonApp.Models.UserModels;
using DotnetSkeletonApp.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DotnetSkeletonApp.Services
{
    public class RoleService(
        ApplicationDbContext _context,
        PermissionService permissionService
    ) : BaseCrudService<ApplicationRole, string, RoleViewModel>(
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

        // protected override async Task<RoleViewModel> AfterCreateAsync(RoleViewModel model, IFormCollection RawFormData)
        protected override async Task<ApplicationRole> AfterCreateAsync(ApplicationRole model, RoleViewModel tviewmodel)
        {

            // var selectedPermissionNames = RawFormData!["choosePermissions[]"].ToList();

            // Console.WriteLine("Choose Permissions: " + string.Join(", ", tviewmodel.ChoosePermissions.Length));

            if (tviewmodel.ChoosePermissions != null && tviewmodel.ChoosePermissions.Length != 0)
            {

                var permissions = _context.Permissions
                    .Where(p => tviewmodel.ChoosePermissions.Contains(p.Name))
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

        protected override async Task<ApplicationRole> AfterUpdateAsync(ApplicationRole model, RoleViewModel tviewmodel)
        {
            var selectedPermissionNames = tviewmodel.ChoosePermissions;

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

        protected override async Task<ApplicationRole> BeforeDeleteAsync(ApplicationRole role)
        {
            // Cek apakah ada user yang masih pakai role ini
            bool hasUsers = await _context.UserRoles.AnyAsync(ur => ur.RoleId == role.Id);
            if (hasUsers)
                throw new Exception("Role is assigned to users, cannot delete");

            // 🗑 Hapus RolePermissions
            var rolePermissions = _context.RolePermissions.Where(rp => rp.RoleId == role.Id);

            _context.RolePermissions.RemoveRange(rolePermissions);

            await _context.SaveChangesAsync();

            return role;
        }
    }
}