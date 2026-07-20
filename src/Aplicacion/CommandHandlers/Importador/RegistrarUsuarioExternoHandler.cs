using Aplicacion.Commands.Importador;
using Aplicacion.Dtos;
using Dominio.Helpers;
using Dominio.Repositories;
using Dominio.Service;
using System.Collections.Generic;

namespace Aplicacion.CommandHandlers.Importador
{
    public class RegistrarUsuarioExternoHandler : AbstractHandler<RegistrarUsuarioExterno>
    {
        private readonly IUsuarioRepository usuarioRepository;
        private readonly IUsuarioExternoRepository usuarioExternoRepository;
        private readonly ICorreoHelper correoHelper;
        private readonly IUnitOfWork unitOfWork;

        public RegistrarUsuarioExternoHandler(
            IUsuarioRepository usuarioRepository,
            IUsuarioExternoRepository usuarioExternoRepository,
            ICorreoHelper correoHelper,
            IUnitOfWork unitOfWork)
        {
            this.usuarioRepository = usuarioRepository;
            this.usuarioExternoRepository = usuarioExternoRepository;
            this.correoHelper = correoHelper;
            this.unitOfWork = unitOfWork;
        }

        public override IResponse Handle(RegistrarUsuarioExterno message)
        {
            var correo = message.Correo?.Trim();
            var nombreCompleto = string.IsNullOrWhiteSpace(message.Apellidos)
                ? message.Nombre?.Trim()
                : $"{message.Nombre?.Trim()} {message.Apellidos?.Trim()}".Trim();

            var usuario = new Dominio.Models.Usuario
            {
                Contrasena = message.Contrasena,
                IdentificadorAcceso = correo,
                Nombre = nombreCompleto,
                DepartamentoId = null
            };
            // Sin roles ERP: el externo solo usa landing/ecommerce tras verificar correo.
            usuario.InicializarExterno(new List<int>());
            usuarioRepository.Create(usuario);

            var perfil = new Dominio.Models.UsuarioExterno
            {
                Nombre = message.Nombre?.Trim(),
                Apellidos = message.Apellidos?.Trim(),
                Correo = correo,
                Identificador = correo,
                Telefono = message.Telefono?.Trim(),
                Celular = message.Telefono?.Trim()
            };
            perfil.RegistrarCuenta();
            usuarioExternoRepository.Create(perfil);

            // Persistir antes del mail: si falla la BD, no se envía verificación engañosa.
            unitOfWork.Save();
            correoHelper.EnviarCorreoParaVerificacion(perfil.Correo, perfil.TokenVerificacion);

            return new OkResponse();
        }
    }
}
