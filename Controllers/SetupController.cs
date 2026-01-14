using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Models;
using DotnetSkeletonApp.Services;
using Microsoft.AspNetCore.Authorization;

namespace DotnetSkeletonApp.Controllers
{
    [Authorize]
    public class SetupController(
        SetupService setupService
    ) : BaseCrudController<Setup, SetupService, Guid, Setup>(
        setupService
    )
    {

    }
}