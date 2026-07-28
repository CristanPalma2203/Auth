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
            var correo = message.Email?.Trim();
            var nombreCompleto = string.IsNullOrWhiteSpace(message.LastName)
                ? message.Name?.Trim()
                : $"{message.Name?.Trim()} {message.LastName?.Trim()}".Trim();

            var usuario = new Dominio.Models.Usuario
            {
                Password = message.Password,
                AccessIdentifier = correo,
                Name = nombreCompleto,
                DepartamentoId = null
            };
            usuario.InicializarExterno(new List<int>());
            usuarioRepository.Create(usuario);

            var perfil = new Dominio.Models.UsuarioExterno
            {
                Name = message.Name?.Trim(),
                LastName = message.LastName?.Trim(),
                Email = correo,
                Identificador = correo,
                Phone = message.Phone?.Trim(),
                Mobile = message.Phone?.Trim()
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
                correoHelper.EnviarCorreoParaVerificacion(perfil.Email, perfil.VerificationToken, baseUrl);
            }
            else
            {
                correoHelper.EnviarCorreoParaVerificacion(perfil.Email, perfil.VerificationToken);
            }

            return new OkResponse();
        }
    }
}
