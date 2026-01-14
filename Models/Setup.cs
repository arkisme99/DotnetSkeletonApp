using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetSkeletonApp.Models
{
    public class Setup : AuditableEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? NameApp { get; set; }
        public string? LogoApp { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }

        [NotMapped]
        public IFormFile? LogoForm { get; set; }
    }
}