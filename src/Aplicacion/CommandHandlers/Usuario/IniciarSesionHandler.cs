using Aplicacion.Commands.Usuario;
using Aplicacion.Dtos;
using Aplicacion.Dtos.Usuario;
using Aplicacion.Mappers;
using Dominio.Especificaciones;
using Dominio.Repositories;
using Dominio.Service;
using Mapster;
using MapsterMapper;

namespace Aplicacion.CommandHandlers.Usuario
{
    public class IniciarSesionHandler : AbstractHandler<IniciarSesion>
    {
        private readonly IUsuarioRepository usuarioRepository;
        private readonly IPermisoRepository permisoRepository;
        private readonly ITokenService tokenService;

        public IniciarSesionHandler(
            IUsuarioRepository usuarioRepository,
            IPermisoRepository permisoRepository,
            ITokenService tokenService)
        {
            this.usuarioRepository = usuarioRepository;
            this.permisoRepository = permisoRepository;
            this.tokenService = tokenService;
        }

        public override IResponse Handle(IniciarSesion message)
        {
            var usuario = usuarioRepository.GetUsuarioConRolPermiso(
                new BuscarUsuarioPorIdentificadorYContrasena(message.Usuario, message.Contrasena));

            var respuesta = UsuarioMappingHelper.ToDtoLogin(usuario, permisoRepository);
            respuesta.Token = tokenService.CrearOtraerToken(usuario);
            return respuesta;
        }
    }
}
