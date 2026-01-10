using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DotnetSkeletonApp.Helpers.Authorization;
using DotnetSkeletonApp.Models;
using DotnetSkeletonApp.Models.UserModels;
using DotnetSkeletonApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetSkeletonApp.Controllers
{
    [Authorize]
    public class RoleController(
        ILogger<BaseCrudController<ApplicationRole, RoleService>> logger,
        RoleService roleService
    ) : BaseCrudController<ApplicationRole, RoleService>(
        logger,
        roleService
        )
    {
        [HasPermission("View_Role")]
        public override IActionResult Index() => base.Index();
        [HasPermission("Create_Role")]
        public override IActionResult Create() => base.Create();

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

    }
}