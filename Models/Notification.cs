using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetSkeletonApp.Models
{
    public class Notification : AuditableEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } = string.Empty; // aspnetusers id
        public string Message { get; set; } = string.Empty;
        public string? FileUrl { get; set; }
        public bool IsRead { get; set; } = false;
        public string MethodName { get; set; } = string.Empty;
    }
}