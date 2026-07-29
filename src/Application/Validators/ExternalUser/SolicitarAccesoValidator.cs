using Application.Commands.ExternalUser;
using Application.Services.Validaciones;
using Domain.Models;
using Domain.Repositories;
using Domain.Service;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Specifications;

namespace Application.Validators.ExternalUser
{
    public class RequestAccessValidator : Validator<RequestAccess>
    {
        private readonly IStoredFileRepository storedFileRepository;
        private readonly IExternalUserRepository externalUserRepository;
        private readonly IAppUserRepository appUserRepository;



        public RequestAccessValidator(IAutenticationHelper autenticationHelper, IStoredFileRepository storedFileRepository,
            IExternalUserRepository externalUserRepository, IAppUserRepository appUserRepository) : base(autenticationHelper)
        {

            RuleFor(x => x).NotEmpty()
                .Must(c => UsuarioNoExiste(c.ExternalUser.Identifier, c.ExternalUser.Email, c))
                .WithMessage("Su combinacion de Nit y Email no existe");
            RuleFor(x => x).NotEmpty()
                .Must(c => UsuarioYaRegistrado(c.ExternalUser.Identifier, c.ExternalUser.Email, c))
                .WithMessage("El ExternalUser ya esta ingresado en el sistema");
            RuleFor(x => x).NotEmpty()
              .Must(c => Tieneusuario(c.ExternalUser.Identifier, c)).WithMessage(("Ya existe un usuario con los roles que ha solicitado"));
            RuleFor(x => x.ExternalUser.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.ExternalUser.Identifier).NotEmpty().WithMessage(("Debe ingresar un numero de identificación"));



            this.storedFileRepository = storedFileRepository;
            this.externalUserRepository = externalUserRepository;
            this.appUserRepository = appUserRepository;
        }

        private bool Tieneusuario(string identifier, RequestAccess importadorAcceso)
        {
            var impotador = externalUserRepository.Filter(new Func<Domain.Models.ExternalUser, bool>(c => c.Identifier == identifier)).FirstOrDefault();
            var appUser = appUserRepository.Filter(new FindUserByIdentifier(identifier));
            if (appUser.Count() == 0) { return true; }
            else
            {
                var user = appUserRepository.GetUserWithRolePermissions(new FindUserByIdentifier(identifier));

                if (impotador != null )
                {
                    return true;

                }
                else { return false; }
            }
        }
        private bool MismoCorreo(string identifier, string Email, RequestAccess importadorAcceso)
        {
            
            var appUser = appUserRepository.Filter(new FindUserByIdentifier(identifier));
            if (appUser.Count() == 0) { return true; }
            else
            {
                var impotador = externalUserRepository.Filter(new Func<Domain.Models.ExternalUser, bool>(c => c.Identifier == identifier)).FirstOrDefault();
                if (impotador != null && Email == impotador.Email)
                { return true; }
                else
                { return false; }
            }
            
            


        }

        private bool UsuarioYaRegistrado(string identifier, string Email, RequestAccess importadorAcceso)
        {
            
            var impotador = externalUserRepository.Filter(new Func<Domain.Models.ExternalUser, bool>(c => c.Identifier == identifier)).FirstOrDefault();
            if (impotador == null)
            {
                return true;
            }
            else if (!impotador.AccessApproved)
            {

                return true;
            }
            else {
                return false;
            }

            

        }
        private bool UsuarioNoExiste(string identifier, string Email, RequestAccess importadorAcceso)
        {

            var impotador = externalUserRepository.Filter(new Func<Domain.Models.ExternalUser, bool>(c => c.Identifier == identifier && c.Email == Email)).FirstOrDefault();

            if (importadorAcceso.ExternalUser.UserExist)
            {

                if (impotador == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
            else
            {
                return true;
            }


        }

        public override IList<string> RequiredPermissions => new List<string>();
    }
}
