using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetSkeletonApp.Models.ViewModels
{
    public class UserViewModel
    {
        public Guid? Id { get; set; }
        public string UserName { get; set; } = null!;
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public IFormFile? Photo { get; set; }

        // Properti ini untuk menampilkan nama file yang sudah ada di DB (saat Edit)
        public string? ExistingPhotoPath { get; set; }
    }
}