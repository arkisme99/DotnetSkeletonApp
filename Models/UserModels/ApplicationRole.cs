using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DotnetSkeletonApp.Models.UserModels
{
    public class ApplicationRole : IdentityRole
    {
        public string? Description { get; set; }
        [Column("created_at", TypeName = "timestamp")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at", TypeName = "timestamp")]
        public DateTime? UpdatedAt { get; set; }

        //soft delete
        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;
        [Column("deleted_at", TypeName = "timestamp")]
        public DateTime? DeletedAt { get; set; }
        public ICollection<ApplicationRolePermission>? RolePermissions { get; set; }
    }
}