using Application.Commands.AppUser;
using Application.Dtos;
using Application.Dtos.AppUser;
using Application.Mappers;
using Domain.Specifications;
using Domain.Repositories;
using Domain.Service;
using Mapster;
using MapsterMapper;

namespace Application.CommandHandlers.AppUser
{
    public class SignInHandler : AbstractHandler<SignIn>
    {
        private readonly IAppUserRepository appUserRepository;
        private readonly IPermissionRepository permissionRepository;
        private readonly ITokenService tokenService;

        public SignInHandler(
            IAppUserRepository appUserRepository,
            IPermissionRepository permissionRepository,
            ITokenService tokenService)
        {
            this.appUserRepository = appUserRepository;
            this.permissionRepository = permissionRepository;
            this.tokenService = tokenService;
        }

        public override IResponse Handle(SignIn message)
        {
            var appUser = appUserRepository.GetUserWithRolePermissions(
                new FindUserByIdentifierAndPassword(message.AppUser, message.Password));

            var respuesta = UserMappingHelper.ToDtoLogin(appUser, permissionRepository);
            respuesta.Token = tokenService.CreateOrGetToken(appUser);
            return respuesta;
        }
    }
}
