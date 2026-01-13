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
        UserManager<ApplicationUser> userManager
    ) : BaseCrudService<ApplicationUser, Guid, UserViewModel>(
        _context
    )
    {
        public readonly UserManager<ApplicationUser> _userManager = userManager;

        //Ternyata identityUser pakainya string, bukan Guid
        public override async Task<ApplicationUser> GetByIdAsync(Guid id)
        {
            var data = await _context.Set<ApplicationUser>().FindAsync(id.ToString());
            return data!;
        }

        public override IQueryable<ApplicationUser> GetQueryAble()
        {
            // Jangan hanya return _context.Set<TModel>()
            var quero = _context.ApplicationUsers
                .Select(u => new ApplicationUser
                {
                    Id = u.Id,
                    FullName = u.FullName!,
                    UserName = u.UserName!,
                    Email = u.Email!,
                    // Builder Anda akan memicu SQL JOIN di sini:
                    RoleNames = (from ur in _context.UserRoles
                                 join r in _context.Roles on ur.RoleId equals r.Id
                                 where ur.UserId == u.Id
                                 select r.Name).ToList()
                });

            var sql = quero.ToQueryString();
            Console.WriteLine("Cek SQL 2: " + sql);

            return quero;
        }

        public async Task<ApplicationUser> UpdateUserAsync(Guid id, ApplicationUser applicationUser)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                /* var user = await _userManager.FindByIdAsync(id.ToString()!) ?? throw new Exception($"User not found");

                //upload foto dahulu
                var fileName = await ProcessUpload(applicationUser.PhotoForm!, "avatar");

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
                Console.WriteLine("Update User {user} Result: {Result}", user.Email, result.Succeeded ? "Success" : "Failed");

                if (!result.Succeeded) throw new Exception("Failed to update user");

                // update password jika ada
                if (!string.IsNullOrWhiteSpace(applicationUser.Password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passResult = await _userManager.ResetPasswordAsync(user, token, applicationUser.Password);

                    if (!passResult.Succeeded)
                        throw new Exception(string.Join(", ", passResult.Errors.Select(e => e.Description)));
                } */

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

        public async Task<ApplicationUser> CreateUserAsync(ApplicationUser applicationUser)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // cek username unik
                /* var existingUser = await _userManager.FindByNameAsync(applicationUser.UserName!);
                if (existingUser != null)
                    throw new Exception("Username already exists");

                //upload foto dahulu
                var fileName = await ProcessUpload(applicationUser.PhotoForm!, "avatar");

                var user = new ApplicationUser
                {
                    UserName = applicationUser.UserName,
                    FullName = applicationUser.FullName,
                    Email = applicationUser.Email ?? applicationUser.UserName,
                    EmailConfirmed = true,
                    Photo = fileName
                };

                var adminPassword = applicationUser.Password;

                var result = await _userManager.CreateAsync(user, adminPassword!);
                Console.Writeln("Create User {user} Result: {Result}", user.Email, result.Succeeded ? "Success" : "Failed");

                if (!result.Succeeded) throw new Exception("Failed to create user"); */

                /* if (applicationUser.Roles != null && applicationUser.Roles.Count > 0)
                {
                    foreach (var role in applicationUser.Roles)
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