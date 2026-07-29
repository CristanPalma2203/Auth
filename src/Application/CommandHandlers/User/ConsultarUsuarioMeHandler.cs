using Application.Commands.AppUser;
using Application.Dtos;
using Application.Dtos.AppUser;
using Application.Exceptions;
using Application.Mappers;
using Domain.Repositories;
using Domain.Service;

namespace Application.CommandHandlers.AppUser
{
    public class GetCurrentUserHandler : AbstractHandler<GetCurrentUser>
    {
        private readonly IAppUserRepository appUserRepository;
        private readonly IPermissionRepository permissionRepository;
        private readonly ITokenService tokenService;

        public GetCurrentUserHandler(
            IAppUserRepository appUserRepository,
            IPermissionRepository permissionRepository,
            ITokenService tokenService)
        {
            this.appUserRepository = appUserRepository;
            this.permissionRepository = permissionRepository;
            this.tokenService = tokenService;
        }

        public override IResponse Handle(GetCurrentUser message)
        {
            var id = tokenService.GetUserId();
            var appUser = appUserRepository.GetUserWithRolePermissions(new Domain.Specifications.FindUserById(id));
            if (appUser == null)
                throw new HttpException(404, "AppUser no encontrado");
            return UserMappingHelper.ToDtoLogin(appUser, permissionRepository);
        }
    }
}
