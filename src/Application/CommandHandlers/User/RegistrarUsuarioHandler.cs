using Application.Commands.AppUser;
using Application.Dtos;
using Application.Dtos.AppUser;
using Application.Exceptions;
using Application.Helpers;
using Domain.Helpers;
using Domain.Repositories;
using Domain.Service;
using MapsterMapper;
using System.Linq;

namespace Application.CommandHandlers.AppUser
{
    public class RegisterUserHandler : AbstractHandler<RegisterUser>
    {
        private readonly IMapper mapper;
        private readonly IAppUserRepository appUserRepository;
        private readonly IEmailHelper correoHelper;
        private readonly IRoleRepository roleRepository;
        private readonly ITenantContext tenantContext;

        public RegisterUserHandler(
            IMapper mapper,
            IEmailHelper correoHelper,
            IAppUserRepository appUserRepository,
            IRoleRepository roleRepository,
            ITenantContext tenantContext)
        {
            this.mapper = mapper;
            this.appUserRepository = appUserRepository;
            this.correoHelper = correoHelper;
            this.roleRepository = roleRepository;
            this.tenantContext = tenantContext;
        }

        public override IResponse Handle(RegisterUser message)
        {
            EnsureRolesDelTenant(message.AppUser.Roles?.Select(c => c.Id).ToList());

            var password = StringHelper.RandomString(7);
            var appUser = mapper.Map<Domain.Models.AppUser>(message.AppUser);
            appUser.Password = password;
            if (!tenantContext.IsPlatformAdmin)
                appUser.TenantId = tenantContext.TenantId;
            appUser.Initialize(Domain.Models.AppUser.internalUserType, message.AppUser.Roles.Select(c => c.Id).ToList());
            appUserRepository.Create(appUser);
            correoHelper.SendUserCreatedEmail(message.AppUser.AccessIdentifier, password, message.AppUser.AccessIdentifier);
            return new OkResponse();
        }

        private void EnsureRolesDelTenant(System.Collections.Generic.IList<int> roleIds)
        {
            if (tenantContext.IsPlatformAdmin || roleIds == null) return;
            foreach (var roleId in roleIds)
            {
                var Roles = roleRepository.GetById(roleId);
                if (Roles == null || Roles.TenantId != tenantContext.TenantId)
                    throw new HttpException(403, "Solo puede asignar roles de su empresa");
            }
        }
    }
}
