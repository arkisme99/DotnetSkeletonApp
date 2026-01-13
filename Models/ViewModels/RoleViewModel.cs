using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Models.UserModels;

namespace DotnetSkeletonApp.Models.ViewModels
{
    public class RoleViewModel : ApplicationRole
    {
        public string[] ChoosePermissions { get; set; } = [];
    }
}