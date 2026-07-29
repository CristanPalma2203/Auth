using Domain.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.Validaciones
{
    public interface IValidatorService
    {
        void AplicarValidador(IMessage message);
    }
}
