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
            RuleFor(x => x.Photo)
                .IsValidImage();

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