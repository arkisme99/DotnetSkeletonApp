using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Models.ViewModels;
using FluentValidation;

namespace DotnetSkeletonApp.Validator
{
    public class UserValidator : AbstractValidator<UserViewModel>
    {
        public UserValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty();

            RuleFor(x => x.FullName)
                .NotEmpty();
            // .WithMessage("Nama tidak boleh kosong");

            // Validasi khusus untuk upload file
            RuleFor(x => x.Photo)
                .IsValidImage();
            // .WithMessage("Silakan pilih foto")
        }
    }
}