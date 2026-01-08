using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Models;

namespace DotnetSkeletonApp.Models.UserModels
{
    public class ApplicationRolePermission : AuditableEntity
    {
        public string? RoleId { get; set; }
        public ApplicationRole? Role { get; set; }
        public Guid PermissionId { get; set; }
        public Permission? Permission { get; set; }
    }
}