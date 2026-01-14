using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Data;
using DotnetSkeletonApp.Models;

namespace DotnetSkeletonApp.Services
{
    public class SetupService(
        ApplicationDbContext context
    ) : BaseCrudService<Setup, Guid, Setup>(
        context
    )
    {

    }
}