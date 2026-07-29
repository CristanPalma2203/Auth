using Application.Commands.ExternalUser;
using Application.Services.Validaciones;
using Domain.Specifications;
using Domain.Models.Rules;
using Domain.Repositories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Application.Validators.ExternalUser
{
    public class InviteExternalUserValidator : Validator<InviteExternalUser>
    {
        private readonly IExternalUserRepository externalUserRepository;
        private readonly IAppUserRepository appUserRepository;

        public InviteExternalUserValidator(IAutenticationHelper autenticationHelper, IExternalUserRepository externalUserRepository, IImportadoresCorreoEnviado importadoresCorreoEnviado, IAppUserRepository appUserRepository) : base(autenticationHelper)
        {
            //RuleFor(x => x).NotEmpty().Must(c => !importadoresCorreoEnviado.VerifyEmailEnviado(c.ExternalUserId))
            //    .WithMessage("El usuario ya fue invitado").Must(c=>NoExisteUsuario(c.ExternalUserId, c)).WithMessage("No puedes volver asignarle roles que ya tiene el appUser");
                  
            this.externalUserRepository = externalUserRepository;
            this.appUserRepository = appUserRepository;
        }

        private bool NoExisteUsuario(int externalUserId, InviteExternalUser iv) {
            var externalUser = externalUserRepository.GetById(externalUserId);
            var appUser = appUserRepository.Filter(new FindUserByIdentifier(externalUser.Identifier));
            var user = appUserRepository.GetUserWithRolePermissions(new FindUserByIdentifier(externalUser.Identifier));
            if (appUser.Count() != 0)
            {
                foreach (var roles in iv.Accesos)
                {
                    foreach (var UsuarioRoles in user.Roles)
                    {

                        if ( UsuarioRoles.RoleId == 2 && roles == 2)
                        {
                            return false;
                        }

                        else if (UsuarioRoles.RoleId == 1 && roles == 1)
                        {
                            

                            return false;
                        }
                        else if (UsuarioRoles.RoleId == 23 && roles == 23)
                        {
                            
                            return false;
                        }
                        else if (UsuarioRoles.RoleId == 37 && roles == 37){return false;}


                    }
                }
                return true;
            }
            return appUser.Count() == 0;
        }
        
        public override IList<string> RequiredPermissions => new List<string> { "manage-external-user", "manage-external-user" };
    }
}
