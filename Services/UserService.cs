using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Data;
using DotnetSkeletonApp.Models.UserModels;
using Microsoft.EntityFrameworkCore;

namespace DotnetSkeletonApp.Services
{
    public class UserService(
        ApplicationDbContext _context
    ) : BaseCrudService<ApplicationUser>(
        _context
    )
    {
        public override IQueryable<ApplicationUser> GetQueryAble()
        {
            // Ini seperti User::with('roles')->get() di Laravel
            return _context.Set<ApplicationUser>()
                           .Include(u => u.UserRoles)
                           //   .ThenInclude(ur => ur.Role)
                           .AsQueryable();
        }
    }
}