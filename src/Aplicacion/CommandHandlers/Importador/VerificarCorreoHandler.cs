using Aplicacion.Commands.Importador;
using Aplicacion.Dtos;
using Dominio.Especificaciones;
using Dominio.Repositories;
using System.Linq;

namespace Aplicacion.CommandHandlers.Importador
{
    public class VerificarCorreoHandler : AbstractHandler<VerificarCorreo>
    {
        private readonly IUsuarioExternoRepository usuarioExternoRepository;
        private readonly IUsuarioRepository usuarioRepository;

        public VerificarCorreoHandler(
            IUsuarioExternoRepository usuarioExternoRepository,
            IUsuarioRepository usuarioRepository)
        {
            this.usuarioExternoRepository = usuarioExternoRepository;
            this.usuarioRepository = usuarioRepository;
        }

        public override IResponse Handle(VerificarCorreo message)
        {
            var perfil = usuarioExternoRepository
                .Filter(new BuscarImportadorPorTokenDeVerificacion(message.Token))
                .FirstOrDefault();

            if (perfil.CorreoVerificado)
            {
                return new OkResponse();
            }

            perfil.VerificarCorreo();
            usuarioExternoRepository.Update(perfil.Id, perfil);

            var usuario = usuarioRepository
                .Filter(new BuscarUsuarioPorIdentificador(perfil.Identificador ?? perfil.Correo))
                .FirstOrDefault();
            if (usuario != null && !usuario.Activo)
            {
                usuario.Enable();
                usuarioRepository.Update(usuario.Id, usuario);
            }

            return new OkResponse();
        }
    }
}
