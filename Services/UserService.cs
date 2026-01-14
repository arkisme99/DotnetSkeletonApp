using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Data;
using DotnetSkeletonApp.Models;
using DotnetSkeletonApp.Models.UserModels;
using DotnetSkeletonApp.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DotnetSkeletonApp.Services
{
    public class UserService(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager
    ) : BaseCrudService<ApplicationUser, string, UserViewModel>(
        context
    )
    {
        public readonly UserManager<ApplicationUser> userManager = userManager;
        public readonly RoleManager<ApplicationRole> roleManager = roleManager;
        public readonly ApplicationDbContext context = context;

        public override IQueryable<ApplicationUser> GetQueryAble()
        {
            var quero = context.ApplicationUsers
                .Select(u => new ApplicationUser
                {
                    Id = u.Id,
                    FullName = u.FullName!,
                    UserName = u.UserName!,
                    Email = u.Email!,
                    Photo = u.Photo!,
                    CreatedAt = u.CreatedAt!,
                    UpdatedAt = u.UpdatedAt!,
                    // Builder Anda akan memicu SQL JOIN di sini:
                    RoleNames = (from ur in context.UserRoles
                                 join r in context.Roles on ur.RoleId equals r.Id
                                 where ur.UserId == u.Id
                                 select r.Name).ToList()
                });

            // var sql = quero.ToQueryString();
            // Console.WriteLine("Cek SQL 2: " + sql);

            return quero;
        }

        protected override async Task<ApplicationUser> BeforeCreateAsync(ApplicationUser applicationUser, UserViewModel tviewmodel)
        {
            var existingUser = await userManager.FindByNameAsync(tviewmodel.UserName!);
            if (existingUser != null)
                throw new Exception("Username already exists");

            //upload foto dahulu
            var fileName = await ProcessUpload(tviewmodel.PhotoForm!, "avatar");

            var user = new ApplicationUser
            {
                UserName = tviewmodel.UserName,
                FullName = tviewmodel.FullName,
                Email = tviewmodel.UserName,
                EmailConfirmed = true,
                Photo = fileName
            };

            return user;
        }

        public override async Task<ApplicationUser> CreateAsync(ApplicationUser tmodel, UserViewModel tviewmodel)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var tuser = await BeforeCreateAsync(tmodel, tviewmodel);

                var adminPassword = tviewmodel.Password;
                // Console.WriteLine($"Check User {tmodel.UserName}");
                var result = await userManager.CreateAsync(tuser, adminPassword!);
                Console.WriteLine($"Create User {tuser.UserName} Result: {result.Succeeded};");

                if (!result.Succeeded) throw new Exception("Failed to create user");

                if (tviewmodel.DataRoles != null && tviewmodel.DataRoles.Length != 0)
                {
                    foreach (var role in tviewmodel.DataRoles!)
                    {
                        var roleEntity = await roleManager.FindByIdAsync(role) ?? throw new Exception($"Role Ono {role} does not exist");

                        // Console.WriteLine($"Masuk ke role {roleEntity.Name} to user {tuser.UserName}");
                        await userManager.AddToRoleAsync(tuser, roleEntity.Name!);
                    }
                }

                await _context.SaveChangesAsync();

                tmodel = await AfterCreateAsync(tmodel, tviewmodel);

                await transaction.CommitAsync();
                return tmodel;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public override async Task<Dictionary<string, object>> EditData(string Id)
        {
            var getRoleByidUserAsync = await GetRoleByidUserAsync(Id);

            return new Dictionary<string, object>
            {
                { "getRoleByidUserAsync", getRoleByidUserAsync }
            };
        }

        public async Task<SelectTwoViewModel[]> GetRoleByidUserAsync(string id)
        {
            // var user = await userManager.FindByIdAsync(id);

            var roles = await _context.UserRoles
                .Where(ur => ur.UserId == id)
                .Join(_context.Roles,
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => new SelectTwoViewModel
                    {
                        Id = r.Id,
                        Text = r.Name!
                    })
                .ToArrayAsync();

            return roles;
        }


        public override async Task<ApplicationUser> UpdateAsync(ApplicationUser applicationUser, UserViewModel tviewmodel)
        {
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var user = await userManager.FindByIdAsync(tviewmodel.Id) ?? throw new Exception($"User not found");

                //upload foto dahulu
                var fileName = await ProcessUpload(tviewmodel.PhotoForm!, "avatar");

                // Update hanya field yang diizinkan agar Password tidak hilang
                user.UserName = tviewmodel.UserName;
                user.Email = tviewmodel.UserName;
                user.FullName = tviewmodel.FullName;
                user.PhoneNumber = tviewmodel.PhoneNumber;

                if (fileName != null)
                {
                    //delete photo lama
                    ProcessDelete(user.Photo!, "avatar");

                    //ubah ke baru
                    user.Photo = fileName;
                }

                var result = await userManager.UpdateAsync(user);
                Console.WriteLine($"Update User {user.UserName} Result: {result.Succeeded};");

                if (!result.Succeeded) throw new Exception("Failed to update user");

                // update password jika ada
                if (!string.IsNullOrWhiteSpace(tviewmodel.Password))
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);
                    var passResult = await userManager.ResetPasswordAsync(user, token, tviewmodel.Password);

                    if (!passResult.Succeeded)
                        throw new Exception(string.Join(", ", passResult.Errors.Select(e => e.Description)));
                }

                // sinkronisasi roles (clear + add ulang)
                if (tviewmodel.DataRoles != null && tviewmodel.DataRoles.Length != 0)
                {
                    var currentRoles = await userManager.GetRolesAsync(user);
                    var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!removeResult.Succeeded)
                        throw new Exception("Failed to clear old roles");

                    foreach (var role in tviewmodel.DataRoles!)
                    {
                        var roleEntity = await roleManager.FindByIdAsync(role) ?? throw new Exception($"Role with id {role} does not exist");
                        await userManager.AddToRoleAsync(user, roleEntity.Name!);
                    }
                }

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