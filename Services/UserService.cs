using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Data;
using DotnetSkeletonApp.Models.UserModels;
using DotnetSkeletonApp.Models.ViewModels;
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

        public async Task<UserViewModel> UpdateUserAsync(Guid id, UserViewModel applicationUser)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString()!) ?? throw new Exception($"User not found");

                //upload foto dahulu
                var fileName = await ProcessUpload(applicationUser.Photo!, "avatar");

                // Update hanya field yang diizinkan agar Password tidak hilang
                user.UserName = applicationUser.UserName ?? applicationUser.Email;
                user.Email = applicationUser.Email;
                user.FullName = applicationUser.FullName;
                user.PhoneNumber = applicationUser.PhoneNumber;

                if (fileName != null)
                {
                    //delete photo lama
                    ProcessDelete(user.Photo!, "avatar");

                    //ubah ke baru
                    user.Photo = fileName;
                }

                var result = await _userManager.UpdateAsync(user);
                _logger.LogInformation("Update User {user} Result: {Result}", user.Email, result.Succeeded ? "Success" : "Failed");

                if (!result.Succeeded) throw new Exception("Failed to update user");

                // update password jika ada
                if (!string.IsNullOrWhiteSpace(applicationUser.Password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passResult = await _userManager.ResetPasswordAsync(user, token, applicationUser.Password);

                    if (!passResult.Succeeded)
                        throw new Exception(string.Join(", ", passResult.Errors.Select(e => e.Description)));
                }

                /*// sinkronisasi roles (clear + add ulang)
                if (dto.Roles != null)
                {
                    var currentRoles = await userManager.GetRolesAsync(user);
                    var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!removeResult.Succeeded)
                        throw new Exception("Failed to clear old roles");

                    foreach (var role in dto.Roles)
                    {
                        var roleEntity = await roleManager.FindByIdAsync(role) ?? throw new Exception($"Role with id {role} does not exist");
                        await userManager.AddToRoleAsync(user, roleEntity.Name!);
                    }
                } */

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