using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Data;
using DotnetSkeletonApp.Models.UserModels;
using Microsoft.EntityFrameworkCore;

namespace DotnetSkeletonApp.Services
{
    public class PermissionService(
        ApplicationDbContext _context
        )
    {
        public async Task<Dictionary<string, List<Permission>>> GetPermissionsAsync()
        {

            var permissions = await _context.Permissions.ToListAsync();

            var grouped = permissions
                .GroupBy(p => p.Name.Split('_').Last().Trim())
                .ToDictionary(g => g.Key, g => g.OrderBy(p => p.Name).ToList());

            return grouped;
        }

        public async Task<List<Permission>> GetRoleWithPermissionsAsync(string roleId)
        {
            var rolePermissions = await _context.RolePermissions
                .Include(rp => rp.Permission)
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.Permission)
                .ToListAsync();

            return rolePermissions!;
        }
    }
}