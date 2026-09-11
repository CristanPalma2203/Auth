using Application.Commands.AppUser;
using Application.Services.Validaciones;
using Domain.Specifications;
using Domain.Models;
using Domain.Repositories;
using Domain.Service;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.Validators
{
    public class SignInValidator : Validator<SignIn>
    {
        private readonly IAppUserRepository appUserRepository;

        public SignInValidator(IAppUserRepository appUserRepository, IAutenticationHelper autenticationHelper) : base(autenticationHelper)
        {
            RuleFor(x => x.AppUser).NotEmpty().WithMessage("Ingrese el identifier")
                .Must(c => !HasInactiveUser(c))
                .WithMessage("AppUser Inactivo");

            RuleFor(x => x.Password).NotEmpty().WithMessage("Ingrese la Contraseña");

            RuleFor(x => x).NotEmpty()
                .Must(c => ValidarCredencialesUsuario(c.AppUser, c.Password))
                .WithMessage("AppUser o contraseña es incorrecto");

            this.appUserRepository = appUserRepository;
            
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

        private bool ValidarCredencialesUsuario(string username, string password)
        {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
                {
                    return false;
                }

                var resultado = appUserRepository.Filter(new FindUserByIdentifierAndPassword(username, password));
                return resultado != null && resultado.Count() > 0;
        }

        public override IList<string> RequiredPermissions => new List<string>();
    }
}