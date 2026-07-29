using Application.Exceptions;
using Application.Services.Validaciones;
using Domain.Service;
using FluentValidation;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

namespace Application.Validators
{
    public abstract class Validator<T> : AbstractValidator<T>, IValidator
    {
        private readonly IAutenticationHelper autenticationHelper;

        public Validator(IAutenticationHelper autenticationHelper)
        {
            this.autenticationHelper = autenticationHelper;
        }
        public abstract IList<string> RequiredPermissions { get; }

        public void Validar(IMessage comando)
        {
            VerificarUsuario();
            ValidarComando(comando);
        }

        public void ValidarComando(IMessage comando)
        {
            var reult = Validate((T)comando);
            if (!reult.IsValid)
            {
                var errores = new List<string>();
                foreach (var failure in reult.Errors)
                {
                    errores.Add(WebUtility.HtmlEncode(failure.ErrorMessage));
                }

                if (errores.Count > 0)
                {
                    throw new HttpException(422, JsonSerializer.Serialize(errores));
                }
            }
        }

        public void VerificarUsuario()
        {
            autenticationHelper.Autenticado(this.RequiredPermissions);
        }
    }
}
