using Application.Commands.ExternalUser;
using Application.Services.Validaciones;
using Domain.Specifications;
using Domain.Repositories;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;

namespace Application.Validators.ExternalUser
{
    public class RegisterExternalUserValidator : Validator<RegisterExternalUser>
    {
        private readonly IAppUserRepository appUserRepository;
        private readonly IExternalUserRepository usuarioExternoRepository;

        public RegisterExternalUserValidator(
            IAutenticationHelper autenticationHelper,
            IAppUserRepository appUserRepository,
            IExternalUserRepository usuarioExternoRepository) : base(autenticationHelper)
        {
            this.appUserRepository = appUserRepository;
            this.usuarioExternoRepository = usuarioExternoRepository;

            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Phone).NotEmpty();
            RuleFor(x => x.Email)
                .Must(CorreoDisponible)
                .WithMessage("Ya existe una cuenta con este correo.");
        }

        private bool CorreoDisponible(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo)) return false;
            var existeUsuario = appUserRepository.Filter(new FindUserByIdentifier(correo)).Any();
            if (existeUsuario) return false;
            var existePerfil = usuarioExternoRepository
                .Filter(c => c.Email != null && c.Email.ToLower().Trim() == correo.ToLower().Trim())
                .Any();
            return !existePerfil;
        }

        public override IList<string> RequiredPermissions => new List<string>();
    }
}
