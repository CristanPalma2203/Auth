using Aplicacion.Commands.Usuario;
using Aplicacion.Dtos;
using Aplicacion.Dtos.Usuario;
using Aplicacion.Exceptions;
using Aplicacion.Helpers;
using Dominio.Helpers;
using Dominio.Repositories;
using Dominio.Service;
using MapsterMapper;
using System.Linq;

namespace Aplicacion.CommandHandlers.Usuario
{
    public class RegistrarUsuarioHandler : AbstractHandler<RegistrarUsuario>
    {
        private readonly IMapper mapper;
        private readonly IUsuarioRepository usuarioRepository;
        private readonly ICorreoHelper correoHelper;
        private readonly IRolRepository rolRepository;
        private readonly ITenantContext tenantContext;

        public RegistrarUsuarioHandler(
            IMapper mapper,
            ICorreoHelper correoHelper,
            IUsuarioRepository usuarioRepository,
            IRolRepository rolRepository,
            ITenantContext tenantContext)
        {
            this.mapper = mapper;
            this.usuarioRepository = usuarioRepository;
            this.correoHelper = correoHelper;
            this.rolRepository = rolRepository;
            this.tenantContext = tenantContext;
        }

        public override IResponse Handle(RegistrarUsuario message)
        {
            EnsureRolesDelTenant(message.Usuario.Roles?.Select(c => c.Id).ToList());

            var contrasena = StringHelper.RandomString(7);
            var usuario = mapper.Map<Dominio.Models.Usuario>(message.Usuario);
            usuario.Contrasena = contrasena;
            if (!tenantContext.IsPlatformAdmin)
                usuario.TenantId = tenantContext.TenantId;
            usuario.Inicializar(Dominio.Models.Usuario.usuarioInterno, message.Usuario.Roles.Select(c => c.Id).ToList());
            usuarioRepository.Create(usuario);
            correoHelper.EnviarCorreoUsuarioCreado(message.Usuario.IdentificadorAcceso, contrasena, message.Usuario.IdentificadorAcceso);
            return new OkResponse();
        }

        private void EnsureRolesDelTenant(System.Collections.Generic.IList<int> roleIds)
        {
            if (tenantContext.IsPlatformAdmin || roleIds == null) return;
            foreach (var roleId in roleIds)
            {
                var rol = rolRepository.GetById(roleId);
                if (rol == null || rol.TenantId != tenantContext.TenantId)
                    throw new HttpException(403, "Solo puede asignar roles de su empresa");
            }
        }
    }
}
