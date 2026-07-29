using Application.Commands.AppUser;
using Application.Services.Validaciones;
using Domain.Specifications;
using Domain.Models.Rules;
using Domain.Repositories;
using Domain.Service;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.Validators
{
    public class ChangePasswordValidator : Validator<ChangePassword>
    {
        private readonly ICambioPassword cambioPassword;
        private readonly ITokenService tokenService;


        public ChangePasswordValidator(IAutenticationHelper autenticationHelper, ICambioPassword cambioPassword, ITokenService TokenService, IAppUserRepository appUserRepository) : base(autenticationHelper)
        {
            RuleFor(x => x.Id).NotEmpty().Must(c => TokenService.VerifyToken())
                .WithMessage("Token Invalido");
            RuleFor(x => x.Password).Must(c =>!string.IsNullOrWhiteSpace(c)).WithMessage("La password es requerida");

            RuleFor(x => x).Must(c=>VerificarPropietario(c.Id)).WithMessage("No puedes cambiar la contraseña por que la cuenta no es propietaria").
                        Must(x => VerificarContrasena(x.Id,x.Password)).WithMessage("No puedes utilizar la misma contraseña");
            RuleFor(x => x).NotEmpty().Must(c => appUserRepository.Filter(new FindInternalUserByIdentifier(c.AccessIdentifier)).Where(s=>s.Id != c.Id).Count() == 0  )
               .WithMessage("Ya existe un usuario con el mismo Email");
            this.cambioPassword = cambioPassword;
            tokenService = TokenService;
        }
        private bool VerificarPropietario(int id) { 
                return id.Equals(tokenService.GetUserId());
        }

        private bool VerificarContrasena(int id, string password) {

                return cambioPassword.verificarPasswor(id, password).Cumple;
        }
        public override IList<string> RequiredPermissions => new List<string> { };
    }
}
