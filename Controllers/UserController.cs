using System;
using System.Collections.Generic;
using System.Linq;
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

        /* public override async Task<IActionResult> Index()
        {
            _logger.LogInformation($"Masuk Ke Index Override {_service!.GetType().Name}");

            var data = await _service.GetAllData();
            return Ok(new { count = data.Count, items = data });
        } */
    }
}