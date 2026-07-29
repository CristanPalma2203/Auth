using Application.Commands.AppUser;
using Application.Dtos;
using Application.Dtos.AppUser;
using Application.Mappers;
using Domain.Repositories;

namespace Application.CommandHandlers.AppUser
{
    class GetUserHandler : AbstractHandler<GetUser>
    {
        private readonly IAppUserRepository appUserRepository;
        private readonly IRoleRepository roleRepository;

        public GetUserHandler(IAppUserRepository appUserRepository, IRoleRepository roleRepository)
        {
            this.appUserRepository = appUserRepository;
            this.roleRepository = roleRepository;
        }

        public override IResponse Handle(GetUser message)
        {
            var appUser = appUserRepository.GetByIdConRoles(message.Id);
            return UserMappingHelper.ToDtoResponse(appUser, roleRepository);
        }
    }
}
