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
                .ToDictionary(g => g.Key, g => g.ToList());

            return grouped;
        }
    }
}