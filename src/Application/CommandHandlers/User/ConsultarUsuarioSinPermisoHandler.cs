using Application.Commands.AppUser;
using Application.Dtos;
using Application.Mappers;
using Domain.Repositories;

namespace Application.CommandHandlers.AppUser
{
    class ListUsersWithoutPermissionHandler : AbstractHandler<ListUsersWithoutPermission>
    {
        private readonly IAppUserRepository appUserRepository;
        private readonly IRoleRepository roleRepository;

        public ListUsersWithoutPermissionHandler(IAppUserRepository appUserRepository, IRoleRepository roleRepository)
        {
            this.appUserRepository = appUserRepository;
            this.roleRepository = roleRepository;
        }

        public override IResponse Handle(ListUsersWithoutPermission message)
        {
            var appUser = appUserRepository.GetByIdConRoles(message.Id);
            return UserMappingHelper.ToDtoResponse(appUser, roleRepository);
        }
    }
}
