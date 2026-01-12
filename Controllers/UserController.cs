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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace DotnetSkeletonApp.Controllers
{
    [Authorize]
    public class UserController(
        ILogger<BaseCrudController<ApplicationUser, UserService>> _logger,
        UserService userService,
        IStringLocalizer<SharedResource> localizer,
        SignInManager<ApplicationUser> signInManager,
        IHttpContextAccessor httpContextAccessor
    ) : BaseCrudController<ApplicationUser, UserService>(
        _logger,
        userService,
        localizer
        )
    {
        public readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        public readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        protected readonly IStringLocalizer<SharedResource> _localizer = localizer;

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
        public override async Task<IActionResult> Edit(Guid Id)
        {
            var breadcrumbs = GetBaseBreadcrumbs();
            breadcrumbs.Add(new BreadcrumbsViewModel($"Edit {ControllerName}", true, ControllerName, "Edit"));
            SetBreadcrumbs([.. breadcrumbs]);

            var DataModel = await _service.GetByIdAsync(Id);
            if (DataModel == null) return NotFound();

            var viewModel = new UserViewModel
            {
                Id = Guid.Parse(DataModel.Id),
                FullName = DataModel.FullName,
                Email = DataModel.Email,
                UserName = DataModel.UserName ?? DataModel.Email!,
                PhoneNumber = DataModel.PhoneNumber,
                ExistingPhotoPath = DataModel.Photo
            };

            return View(viewModel);
        }

        [HasPermission("Edit_User")]
        // [ActionName("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(Guid id, UserViewModel user)
        {
            // if (id.ToString() != user.Id) return NotFound();
            try
            {
                /* if (ModelState.IsValid)
                { */
                // _logger.LogInformation($"Masuk Ke Editx : {user.FullName}");
                await _service.UpdateUserAsync(id, user);

                /* // Cek apakah user yang diedit adalah user yang sedang login
                var currentUserId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (id.ToString() == currentUserId)
                {
                    // Ini akan memperbarui Cookie dengan data terbaru (termasuk Nama/Email)
                    await _signInManager.RefreshSignInAsync(user);
                } */

                TempData["Notify.Type"] = "success";
                TempData["Notify.Message"] = _localizer["PesanUbahSukses"].Value;
                return RedirectToAction(nameof(Index));
                /*  }
                 else
                 {
                     return RedirectToAction(nameof(Index));
                 } */

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


        }
    }
}