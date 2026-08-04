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
            // TenantId nulo significa admin de plataforma, asi que solo el propio admin de
            // plataforma puede decidirlo; cualquier otro usuario queda atado a su empresa.
            var empresaDestino = tenantContext.IsPlatformAdmin
                ? message.AppUser.TenantId
                : tenantContext.TenantId;

            EnsureRolesDelTenant(message.AppUser.Roles?.Select(c => c.Id).ToList(), empresaDestino);

            // Si el administrador define la contraseña, se respeta; si no, se genera y se envía por correo.
            var passwordDefinida = !string.IsNullOrWhiteSpace(message.AppUser.Password);
            var password = passwordDefinida ? message.AppUser.Password : StringHelper.RandomString(7);

            var appUser = mapper.Map<Domain.Models.AppUser>(message.AppUser);
            appUser.Password = password;
            appUser.TenantId = empresaDestino;
            appUser.Initialize(Domain.Models.AppUser.internalUserType, message.AppUser.Roles.Select(c => c.Id).ToList());
            if (passwordDefinida)
                appUser.MustChangePassword = false;

            appUserRepository.Create(appUser);

            if (!passwordDefinida)
                correoHelper.SendUserCreatedEmail(message.AppUser.AccessIdentifier, password, message.AppUser.AccessIdentifier);

            return new OkResponse();
        }

        /// <summary>
        /// Los roles asignados deben pertenecer a la empresa del usuario que se crea.
        /// Si no hay empresa destino, el usuario es de plataforma y usa roles globales.
        /// </summary>
        private void EnsureRolesDelTenant(System.Collections.Generic.IList<int> roleIds, int? empresaDestino)
        {
            if (roleIds == null || !empresaDestino.HasValue) return;

            foreach (var roleId in roleIds)
            {
                var Roles = roleRepository.GetById(roleId);
                if (Roles == null || Roles.TenantId != empresaDestino)
                    throw new HttpException(403, "Solo puede asignar roles de la empresa del usuario");
            }
        }
    }
}
