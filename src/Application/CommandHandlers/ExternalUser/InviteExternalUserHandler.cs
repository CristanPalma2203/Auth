using Application.Commands.ExternalUser;
using Application.Dtos;
using Application.Helpers;
using Domain.Helpers;
using Domain.Models;
using Domain.Repositories;
using Domain.Service;

namespace Application.CommandHandlers.ExternalUser
{
    public class InviteExternalUserHandler : AbstractHandler<InviteExternalUser>
    {
        private readonly IAppUserRepository appUserRepository;
        private readonly IExternalUserRepository externalUserRepository;
        private readonly IEmailHelper correoHelper;
        private readonly ITokenService tokenService;

        public InviteExternalUserHandler(
            IAppUserRepository appUserRepository,
            IExternalUserRepository externalUserRepository,
            IEmailHelper correoHelper,
            ITokenService tokenService)
        {
            this.appUserRepository = appUserRepository;
            this.externalUserRepository = externalUserRepository;
            this.correoHelper = correoHelper;
            this.tokenService = tokenService;
        }

        public override IResponse Handle(InviteExternalUser message)
        {
            message.Accesos = Domain.Models.AppUser.ExternalUserPermissionIds;
            var externalUser = externalUserRepository.GetByIdConDependencias(message.ExternalUserId);
            var password = StringHelper.RandomString(7);

            var appUser = new Domain.Models.AppUser
            {
                Password = password,
                AccessIdentifier = externalUser.Identifier,
                Name = externalUser.Name,
                DepartmentId = 14
            };

            appUser.Initialize(Domain.Models.AppUser.externalUserType, message.Accesos);
            appUserRepository.Create(appUser);
            correoHelper.SendUserCreatedEmail(externalUser.Identifier, password, externalUser.Email);
            externalUser.FinalizarEnvitacion(tokenService.GetUserId(), message.Accesos);
            externalUserRepository.Update(externalUser.Id, externalUser);

            return new OkResponse();
        }
    }
}
