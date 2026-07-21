using Aplicacion.Commands.Importador;
using Aplicacion.Dtos;
using Dominio.Helpers;
using Dominio.Repositories;
using Dominio.Service;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Aplicacion.CommandHandlers.Importador
{
    public class RegistrarUsuarioExternoHandler : AbstractHandler<RegistrarUsuarioExterno>
    {
        private readonly IUsuarioRepository usuarioRepository;
        private readonly IUsuarioExternoRepository usuarioExternoRepository;
        private readonly ICorreoHelper correoHelper;
        private readonly IUnitOfWork unitOfWork;
        private readonly IConfiguration configuration;

        public RegistrarUsuarioExternoHandler(
            IUsuarioRepository usuarioRepository,
            IUsuarioExternoRepository usuarioExternoRepository,
            ICorreoHelper correoHelper,
            IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            this.usuarioRepository = usuarioRepository;
            this.usuarioExternoRepository = usuarioExternoRepository;
            this.correoHelper = correoHelper;
            this.unitOfWork = unitOfWork;
            this.configuration = configuration;
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

            unitOfWork.Save();

            var origen = message.Origen?.Trim().ToLowerInvariant();
            if (origen == "storefront" || origen == "tempora")
            {
                var baseUrl = configuration["AppSettings:VerificarCorreoStorefront"]
                              ?? configuration["AppSettings:VerificarCorreo"]
                              ?? "http://localhost:3001/verificar-correo";
                correoHelper.EnviarCorreoParaVerificacion(perfil.Correo, perfil.TokenVerificacion, baseUrl);
            }
            else
            {
                correoHelper.EnviarCorreoParaVerificacion(perfil.Correo, perfil.TokenVerificacion);
            }

            return new OkResponse();
        }
    }
}
