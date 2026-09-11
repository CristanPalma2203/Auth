using Application.Commands.ExternalUser;
using Application.Services.Validaciones;
using Domain.Models;
using Domain.Repositories;
using Domain.Service;
using Domain.Specifications;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Validators.ExternalUser
{
    public class SignInExternalUserValidator : Validator<SignInExternalUser>
    {
        private readonly IAppUserRepository appUserRepository;

        public SignInExternalUserValidator(
            IAppUserRepository appUserRepository,
            IAutenticationHelper autenticationHelper) : base(autenticationHelper)
        {
            this.appUserRepository = appUserRepository;

            RuleFor(x => x.Identifier).NotEmpty().WithMessage("Ingrese el identifier")
                .Must(c => !HasInactiveUser(c))
                .WithMessage(StorefrontLoginMessages.InactiveUser);

            RuleFor(x => x.Password).NotEmpty().WithMessage("Ingrese la Contraseña");

            RuleFor(x => x)
                .Must(c => CredentialsMatch(c.Identifier, c.Password))
                .WithMessage(StorefrontLoginMessages.InvalidCredentials);
        }

        private bool HasInactiveUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return false;
            }

            return appUserRepository.Filter(new FindUserByIdentifier(username))
                .Any(p => p.IsActive == false);
        }

        private bool CredentialsMatch(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
            {
                return false;
            }

            var resultado = appUserRepository.Filter(new FindUserByIdentifierAndPassword(username, password));
            return resultado != null && resultado.Any();
        }

        public override IList<string> RequiredPermissions => new List<string>();
    }
}
