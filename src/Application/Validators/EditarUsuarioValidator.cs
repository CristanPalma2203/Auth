using Application.Commands.AppUser;
using Application.Services.Validaciones;
using FluentValidation;
using System.Collections.Generic;

namespace Application.Validators
{
    public class EditUserValidator : Validator<EditUser>
    {
        public EditUserValidator(IAutenticationHelper autenticationHelper) : base(autenticationHelper)
        {
            RuleFor(x => x.AppUser).NotNull();
            RuleFor(x => x.AppUser.Name).NotEmpty();
            RuleFor(x => x.AppUser.AccessIdentifier).NotEmpty()
                .MinimumLength(3).WithMessage("El identificador debe tener al menos 3 caracteres");
            RuleFor(x => x.AppUser.Roles).NotEmpty();

            When(x => !string.IsNullOrWhiteSpace(x.AppUser.Dui), () =>
            {
                RuleFor(x => x.AppUser.Dui)
                    .Matches(RegisterUserValidator.DuiPattern)
                    .WithMessage("El DUI debe tener el formato 00000000-0");
            });
            When(x => !string.IsNullOrWhiteSpace(x.AppUser.Nit), () =>
            {
                RuleFor(x => x.AppUser.Nit)
                    .Matches(RegisterUserValidator.NitPattern)
                    .WithMessage("El NIT debe tener el formato 0000-000000-000-0");
            });

            When(x => !string.IsNullOrWhiteSpace(x.AppUser.Password), () =>
            {
                RuleFor(x => x.AppUser.Password).MinimumLength(6)
                    .WithMessage("La contraseña debe tener al menos 6 caracteres");
                RuleFor(x => x.AppUser.ConfirmPassword).Equal(x => x.AppUser.Password)
                    .WithMessage("Las contraseñas no coinciden");
            });
        }

        public override IList<string> RequiredPermissions => new List<string> { "user-edit" };
    }
}
