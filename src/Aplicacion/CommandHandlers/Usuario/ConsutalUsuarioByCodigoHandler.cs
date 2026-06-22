using Aplicacion.Commands.Usuario;
using Aplicacion.Dtos;
using Aplicacion.Mappers;
using Dominio.Especificaciones;
using Dominio.Repositories;

namespace Aplicacion.CommandHandlers.Usuario
{
    class ConsutalUsuarioByCodigoHandler : AbstractHandler<ConsutalUsuarioByCodigo>
    {
        private readonly IRolRepository rolRepository;
        private readonly IUsuarioRepository usuarioRepository;

        public ConsutalUsuarioByCodigoHandler(IRolRepository rolRepository, IUsuarioRepository usuarioRepository)
        {
            this.rolRepository = rolRepository;
            this.usuarioRepository = usuarioRepository;
        }

        public override IResponse Handle(ConsutalUsuarioByCodigo message)
        {
            var usuario = usuarioRepository.GetUsuarioConRolPermiso(
                new BuscarUsuarioPorIdentificadorYCodigo(message.Correo, message.CodigoTemporal));
            return UsuarioMappingHelper.ToDtoResponse(usuario, rolRepository);
        }
    }
}
