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
            RuleFor(x => x.PhotoForm)
                .IsValidImage();

            RuleFor(x => x.DataRoles)
            .NotNull().WithMessage("Silakan pilih minimal satu roles.")
            .NotEmpty().WithMessage("Silakan pilih minimal satu roles.")
            .Must(x => x != null && x.Length > 0).WithMessage("Minimal satu data harus dipilih.");

            RuleSet("Create", () =>
            {
                RuleFor(x => x.Password)
                    .NotEmpty()
                    .MinimumLength(6);
            });

            // Rule khusus untuk Update (Opsional)
            RuleSet("Update", () =>
            {
                RuleFor(x => x.Password)
                    .MinimumLength(6).When(x => !string.IsNullOrEmpty(x.Password));
            });

        }
    }
}