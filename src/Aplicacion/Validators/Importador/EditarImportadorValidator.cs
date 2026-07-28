using Aplicacion.Commands.Importador;
using Aplicacion.Dtos.Importador;
using Aplicacion.Services.Validaciones;
using Dominio.Especificaciones;
using Dominio.Models;
using Dominio.Repositories;
using Dominio.Service;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aplicacion.Validators.Importador
{
    public class EditarImportadorValidator : Validador<EditarImportador>
    {
        private readonly IUsuarioExternoRepository importadorRepository;
        private readonly ITokenService tokenService;
        public EditarImportadorValidator(IAutenticationHelper autenticationHelper,
            IUsuarioExternoRepository importadorRepository,
            ITokenService tokenService) : base(autenticationHelper)
        {

            RuleFor(x => x.Importador).NotEmpty().Must(c => PuedeeditarCorreo(c))
                 .WithMessage("Ya existe un usuario registrado con el correo");
            RuleFor(x => x.Importador.Phone).NotEmpty().WithMessage("Ingresa Un Number Telefonico");
            RuleFor(x => x.Importador.Mobile).NotEmpty().WithMessage("Ingresa Un Number Mobile");
            RuleFor(x => x.Importador.Email).NotEmpty().WithMessage("Ingresa Un Number Email");
            RuleFor(x => x.Importador.Address).NotEmpty().WithMessage("Ingresa Una Dirrecion ");
            //RuleFor(x => x.Importador.EncargadoImportaciones).NotEmpty().WithMessage("Ingresa el encargado");
            this.importadorRepository = importadorRepository;
            this.tokenService = tokenService;
        }
        private bool PuedeeditarCorreo(DtoImportador importador)
        {
            var imp = importadorRepository.GetById(importador.Id.Value);

            var todosConMismoCorreo = importadorRepository.Filter(new Func<Dominio.Models.UsuarioExterno, bool>(p => p.Email == importador.Email));
            if (todosConMismoCorreo.Count() == 0) return true;
            if (todosConMismoCorreo.Count() > 1) return false;
            if (todosConMismoCorreo.Count() == 1 && todosConMismoCorreo.First().Email == imp.Email) return true;
            return true;
        }
        public override IList<string> Permisos => new List<string> { "perfil-importador", "importador-editar", "usuario-externo-editar" };
    }

}
