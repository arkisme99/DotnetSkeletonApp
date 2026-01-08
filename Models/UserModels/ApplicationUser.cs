using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DotnetSkeletonApp.Models.UserModels
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Photo { get; set; }

        DateTime? CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }

        //soft delete
        bool IsDeleted { get; set; }
        DateTime? DeletedAt { get; set; }
    }
}