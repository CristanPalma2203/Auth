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
    public class EditarRolHandler : AbstractHandler<EditarRol>
    {
        private readonly IRolRepository rolRepository;
        private readonly IMapper mapper;
        private readonly IRolPermisoRepository rolPermisoRepository;
        private readonly ICorreoHelper correoHelper;
        private readonly ITokenService tokenService;
        private readonly IUsuarioRepository usuarioRepository;
        private readonly ITenantContext tenantContext;

        public EditarRolHandler(
            IRolRepository rolRepository,
            IUsuarioRepository usuarioRepository,
            ITokenService tokenService,
            ICorreoHelper correoHelper,
            IMapper mapper,
            IRolPermisoRepository rolPermisoRepository,
            ITenantContext tenantContext)
        {
            this.rolRepository = rolRepository;
            this.mapper = mapper;
            this.rolPermisoRepository = rolPermisoRepository;
            this.correoHelper = correoHelper;
            this.tokenService = tokenService;
            this.usuarioRepository = usuarioRepository;
            this.tenantContext = tenantContext;
        }

        public override IResponse Handle(EditarRol message)
        {
            var idUsuario = tokenService.GetIdUsuario();
            var usuario = usuarioRepository.GetById(idUsuario);
            var dbrol = rolRepository.GetByIdConPermisos(message.Id);
            if (dbrol == null)
                throw new HttpException(404, "Rol no encontrado");

            tenantContext.EnsureSameTenantOrPlatform(dbrol.TenantId);
            EnsurePermisosHeredables(message.Rol.Permisos);

            foreach (var item in dbrol.Permisos) rolPermisoRepository.Delete(item.Id);
            dbrol.actualizar(message.Rol.Nombre, message.Rol.Descripcion, message.Rol.Permisos);
            var rolCreado = rolRepository.Update(message.Id, dbrol);
            correoHelper.EnviarCorreoRolCreado(usuario.Nombre, message.Rol.Nombre);
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
