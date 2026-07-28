using Aplicacion.Commands.Importador;
using Aplicacion.Services.Validaciones;
using Dominio.Repositories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aplicacion.Validators.Importador
{
    public class CrearImportadorValidator : Validador<CrearImportador>
    {
        public CrearImportadorValidator(IUsuarioExternoRepository importadorRepository, IAutenticationHelper autenticationHelper) : base(autenticationHelper)
        {
            RuleFor(x => x.Importador.Name).NotEmpty().Must(c => importadorRepository.Filter(new Func<Dominio.Models.UsuarioExterno, bool>(p => p.Name == c)).Count() == 0)
                .WithMessage("Ya existe un Importador con el mismo nombre");
            RuleFor(x => x.Importador.Identifier).NotEmpty().Must(c => importadorRepository.Filter(new Func<Dominio.Models.UsuarioExterno, bool>(p => p.Identifier == c)).Count() == 0)
                .WithMessage("Ya existe un Importador con el mismo Identifier");
            RuleFor(x => x.Importador.Identifier).NotEmpty();
            RuleFor(x => x.Importador.NationalityId).NotEmpty();
            RuleFor(x => x.Importador.DepartmentId).NotEmpty();
            RuleFor(x => x.Importador.MunicipalityId).NotEmpty();
        }
        public override IList<string> Permisos => new List<string> { };
    }
}
