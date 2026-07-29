using Application.Commands.ExternalUser;
using Application.Services.Validaciones;
using Domain.Repositories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.Validators.ExternalUser
{
    public class CreateExternalUserValidator : Validator<CreateExternalUser>
    {
        public CreateExternalUserValidator(IExternalUserRepository externalUserRepository, IAutenticationHelper autenticationHelper) : base(autenticationHelper)
        {
            RuleFor(x => x.ExternalUser.Name).NotEmpty().Must(c => externalUserRepository.Filter(new Func<Domain.Models.ExternalUser, bool>(p => p.Name == c)).Count() == 0)
                .WithMessage("Ya existe un ExternalUser con el mismo nombre");
            RuleFor(x => x.ExternalUser.Identifier).NotEmpty().Must(c => externalUserRepository.Filter(new Func<Domain.Models.ExternalUser, bool>(p => p.Identifier == c)).Count() == 0)
                .WithMessage("Ya existe un ExternalUser con el mismo Identifier");
            RuleFor(x => x.ExternalUser.Identifier).NotEmpty();
            RuleFor(x => x.ExternalUser.NationalityId).NotEmpty();
            RuleFor(x => x.ExternalUser.DepartmentId).NotEmpty();
            RuleFor(x => x.ExternalUser.MunicipalityId).NotEmpty();
        }
        public override IList<string> RequiredPermissions => new List<string> { };
    }
}
