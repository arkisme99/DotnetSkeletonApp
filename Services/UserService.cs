using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Data;
using DotnetSkeletonApp.Models.UserModels;

namespace DotnetSkeletonApp.Services
{
    public class UserService(
        ApplicationDbContext _context
    ) : BaseCrudService<ApplicationUser>(
        _context
    )
    {

    }
}