using Aplicacion.Commands.Importador;
using Aplicacion.Dtos;
using Aplicacion.Helpers;
using Dominio.Helpers;
using Dominio.Models;
using Dominio.Repositories;
using Dominio.Service;

namespace Aplicacion.CommandHandlers.Importador
{
    public class InvitarImportadorHandler : AbstractHandler<InvitarImportador>
    {
        private readonly IUsuarioRepository usuarioRepository;
        private readonly IImportadorRepository importadorRepository;
        private readonly ICorreoHelper correoHelper;
        private readonly ITokenService tokenService;

        public InvitarImportadorHandler(
            IUsuarioRepository usuarioRepository,
            IImportadorRepository importadorRepository,
            ICorreoHelper correoHelper,
            ITokenService tokenService)
        {
            this.usuarioRepository = usuarioRepository;
            this.importadorRepository = importadorRepository;
            this.correoHelper = correoHelper;
            this.tokenService = tokenService;
        }

        public override IResponse Handle(InvitarImportador message)
        {
            message.Accesos = Dominio.Models.Usuario.PermisosUsuarioExterno;
            var importador = importadorRepository.GetByIdConDependencias(message.ImportadorId);
            var contrasena = StringHelper.RandomString(7);

            var usuario = new Dominio.Models.Usuario
            {
                Contrasena = contrasena,
                IdentificadorAcceso = importador.Identificador,
                Nombre = importador.Nombre,
                DepartamentoId = 14
            };

            usuario.Inicializar(Dominio.Models.Usuario.tipoUsuarioImportador, message.Accesos);
            usuarioRepository.Create(usuario);
            correoHelper.EnviarCorreoUsuarioCreado(importador.Identificador, contrasena, importador.Correo);
            importador.FinalizarEnvitacion(tokenService.GetIdUsuario(), message.Accesos);
            importadorRepository.Update(importador.Id, importador);

            return new OkResponse();
        }
    }
}
