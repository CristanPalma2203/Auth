using Aplicacion.Commands.Usuario;
using Aplicacion.Dtos;
using Aplicacion.Mappers;
using Dominio.Repositories;

namespace Aplicacion.CommandHandlers.Usuario
{
    class ConsultarUsuarioSinPermisoHandler : AbstractHandler<ConsultarUsuarioSinPermiso>
    {
        private readonly IUsuarioRepository usuarioRepository;
        private readonly IRolRepository rolRepository;

        public ConsultarUsuarioSinPermisoHandler(IUsuarioRepository usuarioRepository, IRolRepository rolRepository)
        {
            this.usuarioRepository = usuarioRepository;
            this.rolRepository = rolRepository;
        }

        public override IResponse Handle(ConsultarUsuarioSinPermiso message)
        {
            var usuario = usuarioRepository.GetByIdConRoles(message.Id);
            return UsuarioMappingHelper.ToDtoResponse(usuario, rolRepository);
        }
    }
}
