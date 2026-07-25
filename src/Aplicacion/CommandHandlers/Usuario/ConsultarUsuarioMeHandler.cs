using Aplicacion.Commands.Usuario;
using Aplicacion.Dtos;
using Aplicacion.Dtos.Usuario;
using Aplicacion.Exceptions;
using Aplicacion.Mappers;
using Dominio.Repositories;
using Dominio.Service;

namespace Aplicacion.CommandHandlers.Usuario
{
    public class ConsultarUsuarioMeHandler : AbstractHandler<ConsultarUsuarioMe>
    {
        private readonly IUsuarioRepository usuarioRepository;
        private readonly IPermisoRepository permisoRepository;
        private readonly ITokenService tokenService;

        public ConsultarUsuarioMeHandler(
            IUsuarioRepository usuarioRepository,
            IPermisoRepository permisoRepository,
            ITokenService tokenService)
        {
            this.usuarioRepository = usuarioRepository;
            this.permisoRepository = permisoRepository;
            this.tokenService = tokenService;
        }

        public override IResponse Handle(ConsultarUsuarioMe message)
        {
            var id = tokenService.GetIdUsuario();
            var usuario = usuarioRepository.GetUsuarioConRolPermiso(new Dominio.Especificaciones.BuscarUsuarioPorId(id));
            if (usuario == null)
                throw new HttpException(404, "Usuario no encontrado");
            return UsuarioMappingHelper.ToDtoLogin(usuario, permisoRepository);
        }
    }
}
