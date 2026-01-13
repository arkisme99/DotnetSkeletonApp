using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Models.Interface;
using Microsoft.AspNetCore.Identity;

namespace DotnetSkeletonApp.Models.UserModels
{
    public class ApplicationUser : IdentityUser, IAuditableEntity
    {
        public string? FullName { get; set; }
        public string? Photo { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        //soft delete
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        // public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; } = [];
        [NotMapped]
        public List<string> RoleNames { get; set; } = [];
    }
}