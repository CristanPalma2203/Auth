using Aplicacion.Commands.Usuario;
using Aplicacion.Services.Validaciones;
using Dominio.Especificaciones;
using Dominio.Repositories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aplicacion.Validators
{
     class CodigoTemporalValidator : Validador<TemporaryCode>
    {
        private readonly IUsuarioExternoRepository importRepo;
        private readonly IUsuarioRepository user;
        public CodigoTemporalValidator(IAutenticationHelper autenticationHelper, IUsuarioExternoRepository importRepo, IUsuarioRepository user) : base(autenticationHelper)
        {
            RuleFor(x => x.AccessIdentifier).NotEmpty().WithMessage("Ingrese un Email/Identification");
            RuleFor(x => x).NotEmpty()
               .Must(c => ValidarUsuario(c.AccessIdentifier))
               .WithMessage("Identifier / Email no registrado ");
            this.importRepo = importRepo;
            this.user = user;
        }
        private bool ValidarUsuario(string identificador)
        {
            var usuario = user.Filter(new BuscarUsuarioPorIdentificador(identificador));
            return usuario.Count() > 0;

        }
        public override IList<string> Permisos => new List<string>();
    }
}
