using Application.Commands.AppUser;
using Application.Services.Validaciones;
using Domain.Specifications;
using Domain.Repositories;
using Domain.Service;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Validators
{
    public class RegisterUserValidator : Validator<RegisterUser>
    {
        /// <summary>DUI salvadoreño: 00000000-0</summary>
        public const string DuiPattern = @"^\d{8}-\d$";
        /// <summary>NIT salvadoreño: 0000-000000-000-0</summary>
        public const string NitPattern = @"^\d{4}-\d{6}-\d{3}-\d$";

        public RegisterUserValidator(
            IAppUserRepository appUserRepository,
            IAutenticationHelper autenticationHelper,
            ITenantContext tenantContext) : base(autenticationHelper) {
            RuleFor(x => x.AppUser.Name).NotEmpty();
            RuleFor(x => x.AppUser.AccessIdentifier).NotEmpty().EmailAddress()
                .Must(c => appUserRepository.Filter(new FindInternalUserByIdentifier(c)).Count() == 0)
                .WithMessage("Ya existe un usuario con el mismo Email");
            RuleFor(x => x.AppUser.Roles).NotEmpty();

            // Un usuario sin empresa es admin de plataforma, asi que crear uno debe ser
            // una decision explicita y no el resultado de olvidar el campo.
            When(x => tenantContext.IsPlatformAdmin && !x.AppUser.IsPlatformUser, () =>
            {
                RuleFor(x => x.AppUser.TenantId)
                    .NotNull().WithMessage("Debes indicar a qué empresa pertenece el usuario")
                    .GreaterThan(0).WithMessage("Empresa inválida");
            });

            RuleFor(x => x.AppUser.Dui).NotEmpty().WithMessage("El DUI es obligatorio")
                .Matches(DuiPattern).WithMessage("El DUI debe tener el formato 00000000-0");
            RuleFor(x => x.AppUser.Nit).NotEmpty().WithMessage("El NIT es obligatorio")
                .Matches(NitPattern).WithMessage("El NIT debe tener el formato 0000-000000-000-0");

            // La contraseña es opcional: si se omite se genera una y se envía por correo.
            When(x => !string.IsNullOrWhiteSpace(x.AppUser.Password), () =>
            {
                RuleFor(x => x.AppUser.Password).MinimumLength(6)
                    .WithMessage("La contraseña debe tener al menos 6 caracteres");
                RuleFor(x => x.AppUser.ConfirmPassword).Equal(x => x.AppUser.Password)
                    .WithMessage("Las contraseñas no coinciden");
            });
        }

        public override IList<string> RequiredPermissions => new List<string> { "user-create" };
    }
}
