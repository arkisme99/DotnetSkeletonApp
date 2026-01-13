using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using DotnetSkeletonApp.Helpers.Authorization;
using DotnetSkeletonApp.Models;
using DotnetSkeletonApp.Models.UserModels;
using DotnetSkeletonApp.Models.ViewModels;
using DotnetSkeletonApp.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace DotnetSkeletonApp.Controllers
{
    [Authorize]
    public class UserController(
        UserService userService
    ) : BaseCrudController<ApplicationUser, UserService, string, UserViewModel>(
        userService
        )
    {


        [HasPermission("View_User")]
        public override IActionResult Index() => base.Index();
        [HasPermission("Create_User")]
        public override async Task<IActionResult> Create() => await base.Create();

        protected override Dictionary<string, Expression<Func<ApplicationUser, object>>> GetColumnMap()
        {
            return new Dictionary<string, Expression<Func<ApplicationUser, object>>>
            {
                ["id"] = p => p.Id,
                ["fullName"] = p => p.FullName!,
                ["userName"] = p => p.UserName!,
                ["photo"] = p => p.Photo!,
                ["createdAt"] = p => p.CreatedAt!,
                ["updatedAt"] = p => p.UpdatedAt!
            };
        }
        [HasPermission("View_User")]
        public override IActionResult GetDataTable() => base.GetDataTable();


        [HasPermission("Edit_User")]
        public override async Task<IActionResult> Edit(string Id)
        {
            var breadcrumbs = GetBaseBreadcrumbs();
            breadcrumbs.Add(new BreadcrumbsViewModel($"Edit {ControllerName}", true, ControllerName, "Edit"));
            SetBreadcrumbs([.. breadcrumbs]);

            var DataModel = await _service.GetByIdAsync(Id);
            if (DataModel == null) return NotFound();

            var userRoles = await _service.GetRoleByidUserAsync(Id);

            var viewModel = new UserViewModel
            {
                // Id = Guid.Parse(DataModel.Id),
                Id = DataModel.Id,
                FullName = DataModel.FullName,
                Email = DataModel.Email!,
                UserName = DataModel.UserName ?? DataModel.Email!,
                PhoneNumber = DataModel.PhoneNumber,
                Photo = DataModel.Photo,
                DataRoles = userRoles
            };

            return View(viewModel);
        }

        /* [HasPermission("Edit_User")]
        // [ActionName("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(Guid id, UserViewModel user)
        {
            

            try
            {
                // _logger.LogInformation($"Masuk Ke Editx : {user.FullName}");
                await _service.UpdateUserAsync(id, user);

                TempData["Notify.Type"] = "success";
                // TempData["Notify.Message"] = _localizer["PesanUbahSukses"].Value;
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                TempData["Notify.Type"] = "error";
                TempData["Notify.Message"] = ex.Message;
                // return BadRequest(ex.Message);
                // return View(user);
                // return RedirectToAction(nameof(Edit), user);
                return RedirectToAction(nameof(Index));
            }


        } */
    }
}