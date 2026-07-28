using Aplicacion.Commands.Rol;
using Aplicacion.Dtos;
using Aplicacion.Exceptions;
using Dominio.Helpers;
using Dominio.Repositories;
using Dominio.Service;
using MapsterMapper;
using System.Collections.Generic;
using System.Linq;

namespace Aplicacion.CommandHandlers.Rol
{
    public class CrearRolHandler : AbstractHandler<CrearRol>
    {
        private readonly IRolRepository rolRepository;
        private readonly IMapper mapper;
        private readonly ICorreoHelper correoHelper;
        private readonly ITokenService tokenService;
        private readonly IUsuarioRepository usuarioRepository;
        private readonly ITenantContext tenantContext;

        public CrearRolHandler(
            IRolRepository rolRepository,
            IMapper mapper,
            ICorreoHelper correoHelper,
            ITokenService tokenService,
            IUsuarioRepository usuarioRepository,
            ITenantContext tenantContext)
        {
            this.rolRepository = rolRepository;
            this.mapper = mapper;
            this.correoHelper = correoHelper;
            this.tokenService = tokenService;
            this.usuarioRepository = usuarioRepository;
            this.tenantContext = tenantContext;
        }

        public override IResponse Handle(CrearRol message)
        {
            var idUsuario = tokenService.GetIdUsuario();
            var usuario = usuarioRepository.GetById(idUsuario);

            EnsurePermisosHeredables(message.Rol.Permisos);

            var rol = mapper.Map<Dominio.Models.Rol>(message.Rol);
            rol.setFechaCreacion();
            rol.CrearRolPermiso(message.Rol.Permisos);
            rol.IsAssignable = true;
            if (!tenantContext.IsPlatformAdmin)
                rol.TenantId = tenantContext.TenantId;
            var rolCreado = rolRepository.Create(rol);
            correoHelper.EnviarCorreoRolCreado(usuario.Name, message.Rol.Name);
            return mapper.Map<DtoRol>(rolCreado);
        }

        private void EnsurePermisosHeredables(IList<int> permisoIds)
        {
            if (tenantContext.IsPlatformAdmin) return;
            if (permisoIds == null || permisoIds.Count == 0) return;
            var allowed = new HashSet<int>(tokenService.TraerPermisos().Where(p => p != null).Select(p => p.Id));
            if (permisoIds.Any(id => !allowed.Contains(id)))
                throw new HttpException(403, "Solo puede asignar permisos que usted tiene");
        }
    }
}
