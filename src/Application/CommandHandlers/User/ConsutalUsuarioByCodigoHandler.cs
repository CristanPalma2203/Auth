using Application.Commands.AppUser;
using Application.Dtos;
using Application.Mappers;
using Domain.Specifications;
using Domain.Repositories;

namespace Application.CommandHandlers.AppUser
{
    class GetUserByCodeHandler : AbstractHandler<GetUserByCode>
    {
        private readonly IRoleRepository roleRepository;
        private readonly IAppUserRepository appUserRepository;

        public GetUserByCodeHandler(IRoleRepository roleRepository, IAppUserRepository appUserRepository)
        {
            this.roleRepository = roleRepository;
            this.appUserRepository = appUserRepository;
        }

        public override IResponse Handle(GetUserByCode message)
        {
            var appUser = appUserRepository.GetUserWithRolePermissions(
                new FindUserByIdentifierAndCode(message.Email, message.TemporaryCode));
            return UserMappingHelper.ToDtoResponse(appUser, roleRepository);
        }
    }
}
