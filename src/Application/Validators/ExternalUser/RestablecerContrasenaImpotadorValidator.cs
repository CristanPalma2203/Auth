using Application.Commands.ExternalUser;
using Application.Services.Validaciones;
using Domain.Specifications;
using Domain.Repositories;
using Domain.Utilities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.Validators.ExternalUser
{
    public class ResetExternalUserPasswordValidator : Validator<ResetExternalUserPassword>
    {
        private readonly IExternalUserRepository externalUserRepository;
        private readonly IAppUserRepository appUserRepository;
        

        public ResetExternalUserPasswordValidator(IAutenticationHelper autenticationHelper,
            IExternalUserRepository externalUserRepository, IAppUserRepository appUserRepository) : base(autenticationHelper)
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.AppUser).NotEmpty().Must(c=> RegexUtilities.IsValidEmail(c)==false).WithMessage("El usuario no puede ser un correo, para users internos contacta al departamento de IT");
            RuleFor(x => x).Must(c => FindUser(c)).WithMessage("No se ha encontrado un importador con el appUser y correo especificado");
            this.externalUserRepository = externalUserRepository;
            this.appUserRepository = appUserRepository;

        }
        private bool FindUser(ResetExternalUserPassword rc)
        {
            var encuentraUsuario = false;
            var appUser = appUserRepository.Filter(new FindUserByIdentifier(rc.AppUser)).FirstOrDefault();
            if (appUser != null)
            {
                var externalUser = externalUserRepository.Filter(new FindExternalUserByEmailIdentifier(rc.Email, rc.AppUser));
                if (externalUser.Count() > 0)encuentraUsuario = true;
            }
            return encuentraUsuario;

        }

        public override IList<string> RequiredPermissions => new List<string>();
    }
}
