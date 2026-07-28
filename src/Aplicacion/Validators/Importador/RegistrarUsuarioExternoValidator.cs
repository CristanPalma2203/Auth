using Aplicacion.Commands.Importador;
using Aplicacion.Services.Validaciones;
using Dominio.Especificaciones;
using Dominio.Repositories;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;

namespace Aplicacion.Validators.Importador
{
    public class RegistrarUsuarioExternoValidator : Validador<RegistrarUsuarioExterno>
    {
        private readonly IUsuarioRepository usuarioRepository;
        private readonly IUsuarioExternoRepository usuarioExternoRepository;

        public RegistrarUsuarioExternoValidator(
            IAutenticationHelper autenticationHelper,
            IUsuarioRepository usuarioRepository,
            IUsuarioExternoRepository usuarioExternoRepository) : base(autenticationHelper)
        {
            this.usuarioRepository = usuarioRepository;
            this.usuarioExternoRepository = usuarioExternoRepository;

            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Phone).NotEmpty();
            RuleFor(x => x.Email)
                .Must(CorreoDisponible)
                .WithMessage("Ya existe una cuenta con este correo.");
        }

        private bool CorreoDisponible(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo)) return false;
            var existeUsuario = usuarioRepository.Filter(new BuscarUsuarioPorIdentificador(correo)).Any();
            if (existeUsuario) return false;
            var existePerfil = usuarioExternoRepository
                .Filter(c => c.Email != null && c.Email.ToLower().Trim() == correo.ToLower().Trim())
                .Any();
            return !existePerfil;
        }

        public override IList<string> Permisos => new List<string>();
    }
}
