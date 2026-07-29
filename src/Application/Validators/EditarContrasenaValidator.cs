using Application.Commands.AppUser;
using Application.Services.Validaciones;
using Domain.Models.Rules;
using Domain.Repositories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Validators
{
    public class EditPasswordValidator : Validator<EditPassword>
    {
        private readonly ICambioPassword cambioPassword;


        public EditPasswordValidator(IAutenticationHelper autenticationHelper, ICambioPassword cambioPassword, IAppUserRepository appUserRepository) : base(autenticationHelper)
        {
            RuleFor(x => x.Password).Must(c => !string.IsNullOrWhiteSpace(c)).WithMessage("La password es requerida");

            RuleFor(x => x).Must(x => VerificarContrasena(x.Id, x.Password)).WithMessage("No puedes utilizar la misma contraseña");
            this.cambioPassword = cambioPassword;
        }
        private bool VerificarContrasena(int id, string password)
        {

            return cambioPassword.verificarPasswor(id, password).Cumple;
        }
        public override IList<string> RequiredPermissions => new List<string> { };
    }
}
