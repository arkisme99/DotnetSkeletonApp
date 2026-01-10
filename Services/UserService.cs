using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Data;
using DotnetSkeletonApp.Models.UserModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DotnetSkeletonApp.Services
{
    public class UserService(
        ApplicationDbContext _context,
        UserManager<ApplicationUser> userManager,
        ILogger<ApplicationUser> logger
    ) : BaseCrudService<ApplicationUser>(
        _context
    )
    {
        public readonly UserManager<ApplicationUser> _userManager = userManager;
        public readonly ILogger<ApplicationUser> _logger = logger;

        public override IQueryable<ApplicationUser> GetQueryAble()
        {
            // Ini seperti User::with('roles')->get() di Laravel
            return _context.Set<ApplicationUser>()
                           .Include(u => u.UserRoles)
                           //   .ThenInclude(ur => ur.Role)
                           .AsQueryable();
        }

        //Ternyata identityUser pakainya string, bukan Guid
        public override async Task<ApplicationUser> GetByIdAsync(Guid id)
        {
            var data = await _context.Set<ApplicationUser>().FindAsync(id.ToString());
            return data!;
        }

        public override async Task<ApplicationUser> UpdateAsync(ApplicationUser applicationUser)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _userManager.FindByIdAsync(applicationUser.Id.ToString()) ?? throw new Exception("User not found");

                // Update hanya field yang diizinkan agar Password tidak hilang
                user.UserName = applicationUser.UserName ?? applicationUser.Email;
                user.Email = applicationUser.Email;
                user.FullName = applicationUser.FullName;
                user.PhoneNumber = applicationUser.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);
                _logger.LogInformation("Update User {user} Result: {Result}", user.Email, result.Succeeded ? "Success" : "Failed");

                if (!result.Succeeded) throw new Exception("Failed to update user");

                await transaction.CommitAsync();

                return applicationUser;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}