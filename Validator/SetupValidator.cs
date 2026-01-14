using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Models;
using FluentValidation;

namespace DotnetSkeletonApp.Validator
{
    public class SetupValidator : AbstractValidator<Setup>
    {
        public SetupValidator()
        {
            RuleFor(x => x.NameApp).MaximumLength(100).NotEmpty();
            RuleFor(x => x.LogoForm)
                .IsValidImage();
        }
    }
}