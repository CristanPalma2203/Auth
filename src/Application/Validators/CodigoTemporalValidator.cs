using Application.Commands.AppUser;
using Application.Services.Validaciones;
using Domain.Specifications;
using Domain.Repositories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.Validators
{
     class TemporaryCodeValidator : Validator<TemporaryCode>
    {
        private readonly IExternalUserRepository importRepo;
        private readonly IAppUserRepository user;
        public TemporaryCodeValidator(IAutenticationHelper autenticationHelper, IExternalUserRepository importRepo, IAppUserRepository user) : base(autenticationHelper)
        {
            RuleFor(x => x.AccessIdentifier).NotEmpty().WithMessage("Ingrese un Email/Identification");
            RuleFor(x => x).NotEmpty()
               .Must(c => ValidarUsuario(c.AccessIdentifier))
               .WithMessage("Identifier / Email no registrado ");
            this.importRepo = importRepo;
            this.user = user;
        }
        private bool ValidarUsuario(string identifier)
        {
            var appUser = user.Filter(new FindUserByIdentifier(identifier));
            return appUser.Count() > 0;

        }
        public override IList<string> RequiredPermissions => new List<string>();
    }
}
