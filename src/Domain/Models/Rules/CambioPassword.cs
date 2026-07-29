using Domain.Repositories;
using FluentValidation;
using FluentValidation.Resources;
using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Rules
{
    public class CambioPassword : ICambioPassword
    {
        private readonly IAppUserRepository appUserRepository;

        public CambioPassword(IAppUserRepository appUserRepository) {
            this.appUserRepository = appUserRepository;
        }
        public IReglaRespuesta verificarPasswor(int idUsuario, string passwor)
        {
            var respuesta = new RegalRespuestaBasica { Cumple=true};
            var user = this.appUserRepository.GetById(idUsuario);
            if (user is null) {
                respuesta.Cumple = false;
                return respuesta;
            }
            if (AppUser.getPassword(passwor).Equals(user.Password)) respuesta.Cumple = false;
            return respuesta;
        }
    }

}
