using Aplicacion.Commands.Importador;
using Aplicacion.Services.Validaciones;
using Dominio.Models;
using Dominio.Repositories;
using Dominio.Service;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.Especificaciones;

namespace Aplicacion.Validators.Importador
{
    public class SolicitarAccesoValidator : Validador<SolicitarAcceso>
    {
        private readonly IArchivoRepository archivoRepository;
        private readonly IUsuarioExternoRepository importadorRepository;
        private readonly IUsuarioRepository usuarioRepository;



        public SolicitarAccesoValidator(IAutenticationHelper autenticationHelper, IArchivoRepository archivoRepository,
            IUsuarioExternoRepository importadorRepository, IUsuarioRepository usuarioRepository) : base(autenticationHelper)
        {

            RuleFor(x => x).NotEmpty()
                .Must(c => UsuarioNoExiste(c.Importador.Identifier, c.Importador.Email, c))
                .WithMessage("Su combinacion de Nit y Email no existe");
            RuleFor(x => x).NotEmpty()
                .Must(c => UsuarioYaRegistrado(c.Importador.Identifier, c.Importador.Email, c))
                .WithMessage("El Importador ya esta ingresado en el sistema");
            RuleFor(x => x).NotEmpty()
              .Must(c => Tieneusuario(c.Importador.Identifier, c)).WithMessage(("Ya existe un usuario con los roles que ha solicitado"));
            RuleFor(x => x.Importador.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Importador.Identifier).NotEmpty().WithMessage(("Debe ingresar un numero de identificación"));



            this.archivoRepository = archivoRepository;
            this.importadorRepository = importadorRepository;
            this.usuarioRepository = usuarioRepository;
        }

        private bool Tieneusuario(string identificador, SolicitarAcceso importadorAcceso)
        {
            var impotador = importadorRepository.Filter(new Func<Dominio.Models.UsuarioExterno, bool>(c => c.Identifier == identificador)).FirstOrDefault();
            var usuario = usuarioRepository.Filter(new BuscarUsuarioPorIdentificador(identificador));
            if (usuario.Count() == 0) { return true; }
            else
            {
                var user = usuarioRepository.GetUsuarioConRolPermiso(new BuscarUsuarioPorIdentificador(identificador));

                if (impotador != null )
                {
                    return true;

                }
                else { return false; }
            }
        }
        private bool MismoCorreo(string identificador, string Email, SolicitarAcceso importadorAcceso)
        {
            
            var usuario = usuarioRepository.Filter(new BuscarUsuarioPorIdentificador(identificador));
            if (usuario.Count() == 0) { return true; }
            else
            {
                var impotador = importadorRepository.Filter(new Func<Dominio.Models.UsuarioExterno, bool>(c => c.Identifier == identificador)).FirstOrDefault();
                if (impotador != null && Email == impotador.Email)
                { return true; }
                else
                { return false; }
            }
            
            


        }

        private bool UsuarioYaRegistrado(string identificador, string Email, SolicitarAcceso importadorAcceso)
        {
            
            var impotador = importadorRepository.Filter(new Func<Dominio.Models.UsuarioExterno, bool>(c => c.Identifier == identificador)).FirstOrDefault();
            if (impotador == null)
            {
                return true;
            }
            else if (!impotador.AccessApproved)
            {

                return true;
            }
            else {
                return false;
            }

            

        }
        private bool UsuarioNoExiste(string identificador, string Email, SolicitarAcceso importadorAcceso)
        {

            var impotador = importadorRepository.Filter(new Func<Dominio.Models.UsuarioExterno, bool>(c => c.Identifier == identificador && c.Email == Email)).FirstOrDefault();

            if (importadorAcceso.Importador.UserExist)
            {

                if (impotador == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
            else
            {
                return true;
            }


        }

        public override IList<string> Permisos => new List<string>();
    }
}
