using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DotnetSkeletonApp.Helpers.Authorization;
using DotnetSkeletonApp.Models;
using DotnetSkeletonApp.Models.UserModels;
using DotnetSkeletonApp.Models.ViewModels;
using DotnetSkeletonApp.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace DotnetSkeletonApp.Controllers
{
    [Authorize]
    public class RoleController(
        RoleService roleService
    ) : BaseCrudController<ApplicationRole, RoleService, string, RoleViewModel>(
        roleService
        )
    {
        public readonly RoleService roleService = roleService;
        [HasPermission("View_Role")]
        public override IActionResult Index() => base.Index();

        [HasPermission("Create_Role")]
        public override async Task<IActionResult> Create() => await base.Create();

        [HasPermission("Edit_Role")]
        public override async Task<IActionResult> Edit(string id) => await base.Edit(id);

        [HasPermission("Delete_Role")]
        public override async Task<IActionResult> Delete(string Id) => await base.Delete(Id);

        [HasPermission("MultiDelete_Role")]
        public override async Task<IActionResult> MultiDelete(string datahapus) => await base.MultiDelete(datahapus);

        protected override Dictionary<string, Expression<Func<ApplicationRole, object>>> GetColumnMap()
        {
            return new Dictionary<string, Expression<Func<ApplicationRole, object>>>
            {
                ["id"] = p => p.Id,
                ["name"] = p => p.Name!,
                ["normalizedName"] = p => p.NormalizedName!,
                ["description"] = p => p.Description!,
                ["createdAt"] = p => p.CreatedAt!,
                ["updatedAt"] = p => p.UpdatedAt!
            };
        }
        [HasPermission("View_Role")]
        public override IActionResult GetDataTable() => base.GetDataTable();
        [HasPermission("View_Role")]
        public async Task<IActionResult> GetSelect(string? q)
        {
            var query = roleService.GetQueryAble();

            if (!string.IsNullOrEmpty(q))
            {
                // Filter seperti Select2 "search"
                query = query.Where(r => r.Name!.Contains(q)).Take(5);
            }
            else
            {
                // Default ambil 5 data pertama
                query = query.Take(5);
            }

            var roles = await query
                .Select(r => new SelectTwoViewModel
                {
                    Id = r.Id,         // biasanya string (GUID) kalau pakai IdentityRole
                    Text = r.Name!
                })
                .ToListAsync();

            return Ok(roles);
        }

    }
}