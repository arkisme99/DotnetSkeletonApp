using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Models.UserModels;

namespace DotnetSkeletonApp.Models.ViewModels
{
    public class UserViewModel : ApplicationUser
    {
        public string[] DataRoles { get; set; } = [];
        public string? Password { get; set; }
        public IFormFile? PhotoForm { get; set; }
    }
}