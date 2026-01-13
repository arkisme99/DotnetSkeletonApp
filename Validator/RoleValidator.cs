using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Models.ViewModels;
using FluentValidation;

namespace DotnetSkeletonApp.Validator
{
    public class RoleValidator : AbstractValidator<RoleViewModel>
    {
        public RoleValidator()
        {
            RuleFor(x => x.Name).NotEmpty();

            RuleFor(x => x.ChoosePermissions)
            .NotNull().WithMessage("Silakan pilih minimal satu hak akses.")
            .NotEmpty().WithMessage("Silakan pilih minimal satu hak akses.")
            .Must(x => x != null && x.Length > 0).WithMessage("Minimal satu data harus dipilih.");
        }
    }
}