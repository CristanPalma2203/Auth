using Application.Commands.ExternalUser;
using Application.Dtos.ExternalUser;
using Application.Services.Validaciones;
using Domain.Specifications;
using Domain.Models;
using Domain.Repositories;
using Domain.Service;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.Validators.ExternalUser
{
    public class EditExternalUserValidator : Validator<EditExternalUser>
    {
        private readonly IExternalUserRepository externalUserRepository;
        private readonly ITokenService tokenService;
        public EditExternalUserValidator(IAutenticationHelper autenticationHelper,
            IExternalUserRepository externalUserRepository,
            ITokenService tokenService) : base(autenticationHelper)
        {

            RuleFor(x => x.ExternalUser).NotEmpty().Must(c => PuedeeditarCorreo(c))
                 .WithMessage("Ya existe un usuario registrado con el correo");
            RuleFor(x => x.ExternalUser.Phone).NotEmpty().WithMessage("Ingresa Un Number Telefonico");
            RuleFor(x => x.ExternalUser.Mobile).NotEmpty().WithMessage("Ingresa Un Number Mobile");
            RuleFor(x => x.ExternalUser.Email).NotEmpty().WithMessage("Ingresa Un Number Email");
            RuleFor(x => x.ExternalUser.Address).NotEmpty().WithMessage("Ingresa Una Dirrecion ");
            //RuleFor(x => x.ExternalUser.EncargadoImportaciones).NotEmpty().WithMessage("Ingresa el encargado");
            this.externalUserRepository = externalUserRepository;
            this.tokenService = tokenService;
        }
        private bool PuedeeditarCorreo(ExternalUserDto externalUser)
        {
            var imp = externalUserRepository.GetById(externalUser.Id.Value);

            var todosConMismoCorreo = externalUserRepository.Filter(new Func<Domain.Models.ExternalUser, bool>(p => p.Email == externalUser.Email));
            if (todosConMismoCorreo.Count() == 0) return true;
            if (todosConMismoCorreo.Count() > 1) return false;
            if (todosConMismoCorreo.Count() == 1 && todosConMismoCorreo.First().Email == imp.Email) return true;
            return true;
        }
        public override IList<string> RequiredPermissions => new List<string> { "external-user-profile", "external-user-edit", "external-user-edit" };
    }

}
