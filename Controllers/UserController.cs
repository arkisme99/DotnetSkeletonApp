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
    public class UserController(
        ILogger<BaseCrudController<ApplicationUser, UserService>> logger,
        UserService userService
    ) : BaseCrudController<ApplicationUser, UserService>(
        logger,
        userService
        )
    {
        [HasPermission("View_User")]
        public override IActionResult Index() => base.Index();
        [HasPermission("Create_User")]
        public override IActionResult Create() => base.Create();
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
    }
}