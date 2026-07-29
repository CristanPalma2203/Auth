using Domain.Service;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Validators
{
    public interface IValidator
    {
       void VerificarUsuario();
        void ValidarComando(IMessage comando);
        void Validar(IMessage comando);
        IList<string> RequiredPermissions { get; }
      
    }
}
