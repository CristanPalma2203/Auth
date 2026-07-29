using Application.Commands.ExternalUser;
using Application.Services.Validaciones;
using Domain.Specifications;
using Domain.Repositories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.Validators.ExternalUser
{
    public class VerifyEmailValidator : Validator<VerifyEmail>
    {
        private readonly IExternalUserRepository externalUserRepository;

        public VerifyEmailValidator(IAutenticationHelper autenticationHelper, IExternalUserRepository externalUserRepository) : base(autenticationHelper)
        {
            RuleFor(x => x.Token).NotEmpty().Must(x=>ExisteSolicitud(x)).WithMessage("No encontramos registro de la solicitud.");
            this.externalUserRepository = externalUserRepository;
        }

        private bool ExisteSolicitud(string token) {
            var solicitud = externalUserRepository.Filter(new FindExternalUserByVerificationToken(token));
            return solicitud.Count() == 1;
        }

        public override IList<string> RequiredPermissions => new List<string>();
    }
}
