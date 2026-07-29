using Application.Commands.AppUser;
using Application.Services.Validaciones;
using Domain.Specifications;
using Domain.Repositories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Validators
{
    public class RegisterUserValidator : Validator<RegisterUser>
    {
        public RegisterUserValidator(IAppUserRepository appUserRepository, IAutenticationHelper autenticationHelper) : base(autenticationHelper) {
            RuleFor(x => x.AppUser.Name).NotEmpty();
            RuleFor(x => x.AppUser.AccessIdentifier).NotEmpty().Must(c => appUserRepository.Filter(new FindInternalUserByIdentifier(c)).Count() == 0)
                .WithMessage("Ya existe un usuario con el mismo Email");
            RuleFor(x => x.AppUser.Roles).NotEmpty();
            RuleFor(x => x.AppUser.DepartmentId).NotEmpty();
        }

        public override IList<string> RequiredPermissions => new List<string> { "user-create" };
    }
}
