using Application.Commands.ExternalUser;
using Application.Dtos;
using Domain.Specifications;
using Domain.Repositories;
using System.Linq;

namespace Application.CommandHandlers.ExternalUser
{
    public class VerifyEmailHandler : AbstractHandler<VerifyEmail>
    {
        private readonly IExternalUserRepository usuarioExternoRepository;
        private readonly IAppUserRepository appUserRepository;

        public VerifyEmailHandler(
            IExternalUserRepository usuarioExternoRepository,
            IAppUserRepository appUserRepository)
        {
            this.usuarioExternoRepository = usuarioExternoRepository;
            this.appUserRepository = appUserRepository;
        }

        public override IResponse Handle(VerifyEmail message)
        {
            var perfil = usuarioExternoRepository
                .Filter(new FindExternalUserByVerificationToken(message.Token))
                .FirstOrDefault();

            if (perfil.EmailVerified)
            {
                return new OkResponse();
            }

            perfil.VerifyEmail();
            usuarioExternoRepository.Update(perfil.Id, perfil);

            var appUser = appUserRepository
                .Filter(new FindUserByIdentifier(perfil.Identifier ?? perfil.Email))
                .FirstOrDefault();
            if (appUser != null && !appUser.IsActive)
            {
                appUser.Enable();
                appUserRepository.Update(appUser.Id, appUser);
            }

            return new OkResponse();
        }
    }
}
