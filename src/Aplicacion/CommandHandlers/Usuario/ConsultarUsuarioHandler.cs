using Aplicacion.Commands.Usuario;
using Aplicacion.Dtos;
using Aplicacion.Dtos.Usuario;
using Aplicacion.Mappers;
using Dominio.Repositories;

namespace Aplicacion.CommandHandlers.Usuario
{
    class ConsultarUsuarioHandler : AbstractHandler<ConsultarUsuario>
    {
        private readonly IUsuarioRepository usuarioRepository;
        private readonly IRolRepository rolRepository;

        public ConsultarUsuarioHandler(IUsuarioRepository usuarioRepository, IRolRepository rolRepository)
        {
            this.usuarioRepository = usuarioRepository;
            this.rolRepository = rolRepository;
        }

        public override IResponse Handle(ConsultarUsuario message)
        {
            var usuario = usuarioRepository.GetByIdConRoles(message.Id);
            return UsuarioMappingHelper.ToDtoResponse(usuario, rolRepository);
        }
    }
}
