using Application.Validators;
using Domain.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.Services.Validaciones
{
    public class ValidatorService : IValidatorService
    {
        private readonly IEnumerable<IValidator> validadors;

        public ValidatorService(IEnumerable<IValidator> validadors)
        {
            this.validadors = validadors;
        }
        public void AplicarValidador(IMessage message)
        {
            var instace = validadors.FirstOrDefault(c => c.GetType().Name == message.GetType().Name + "Validator");
            if (instace == null)
            {
                throw new NotImplementedException("Validaciones no creadas para el comando: " + message.GetType().Name);
            }
            instace.Validar(message);
        }
    }
}
