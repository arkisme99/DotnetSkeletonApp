using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Helpers.Authorization;
using DotnetSkeletonApp.Models;
using DotnetSkeletonApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetSkeletonApp.Controllers
{
    [Authorize]
    public class SetupController(
        SetupService setupService
    ) : BaseCrudController<Setup, SetupService, Guid, Setup>(
        setupService
    )
    {
        [HasPermission("View_Setup")]
        public override IActionResult Index() => base.Index();

        [HasPermission("Create_Setup")]
        public override async Task<IActionResult> Create() => await base.Create();

        [HasPermission("Edit_Setup")]
        public override async Task<IActionResult> Edit(Guid Id) => await base.Edit(Id);

        [HasPermission("Delete_Setup")]
        public override async Task<IActionResult> Delete(Guid Id) => await base.Delete(Id);

        [HasPermission("MultiDelete_Setup")]
        public override async Task<IActionResult> MultiDelete(string datahapus) => await base.MultiDelete(datahapus);
    }
}