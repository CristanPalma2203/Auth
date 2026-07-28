using Aplicacion.Commands.Usuario;
using Aplicacion.Services.Validaciones;
using Dominio.Especificaciones;
using Dominio.Repositories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aplicacion.Validators
{
    public class RegistrarUsuarioValidator : Validador<RegistrarUsuario>
    {
        public RegistrarUsuarioValidator(IUsuarioRepository usuarioRepository, IAutenticationHelper autenticationHelper) : base(autenticationHelper) {
            RuleFor(x => x.Usuario.Name).NotEmpty();
            RuleFor(x => x.Usuario.AccessIdentifier).NotEmpty().Must(c => usuarioRepository.Filter(new BuscarUsuarioInternoPorIdentificador(c)).Count() == 0)
                .WithMessage("Ya existe un usuario con el mismo Email");
            RuleFor(x => x.Usuario.Roles).NotEmpty();
            RuleFor(x => x.Usuario.DepartmentId).NotEmpty();
        }

        public override IList<string> Permisos => new List<string> { "usuario-crear" };
    }
}
