using Application.Commands.ExternalUser;
using Application.Services.Validaciones;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Validators.ExternalUser
{
    public class ListExternalUsersValidator : Validator<ListExternalUsers>
    {
        public ListExternalUsersValidator(IAutenticationHelper autenticationHelper) : base(autenticationHelper)
        {
        }

        public override IList<string> RequiredPermissions => new List<string> { "listar-importadores", "external-user-list", "importador-semilla-crear", "crear-establecimiento-salud-animal", "proveedor-fertilizante-crear" };
    }
}
