using Application.Commands.ExternalUser;
using Application.Services.Validaciones;
using Domain.Repositories;
using Domain.Specifications;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;

namespace Application.Validators.ExternalUser
{
    public class ResetExternalUserPasswordValidator : Validator<ResetExternalUserPassword>
    {
        private readonly IExternalUserRepository externalUserRepository;
        private readonly IAppUserRepository appUserRepository;

        public ResetExternalUserPasswordValidator(
            IAutenticationHelper autenticationHelper,
            IExternalUserRepository externalUserRepository,
            IAppUserRepository appUserRepository) : base(autenticationHelper)
        {
            this.externalUserRepository = externalUserRepository;
            this.appUserRepository = appUserRepository;

            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            // Compradores Tempora: AccessIdentifier = correo. Antes se bloqueaba email.
            RuleFor(x => x.AppUser).NotEmpty();
            RuleFor(x => x)
                .Must(FindUser)
                .WithMessage("No se encontro un comprador con ese usuario y correo.");
        }

        private bool FindUser(ResetExternalUserPassword rc)
        {
            var appUser = appUserRepository.Filter(new FindUserByIdentifier(rc.AppUser)).FirstOrDefault();
            if (appUser == null) return false;

            return externalUserRepository
                .Filter(new FindExternalUserByEmailIdentifier(rc.Email, rc.AppUser))
                .Any();
        }

        public override IList<string> RequiredPermissions => new List<string>();
    }
}
