using Application.Commands.Role;
using Application.Services.Validaciones;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Validators
{
    public class ListRolesUnpagedValidator : Validator<ListRolesUnpaged>
    {
        public ListRolesUnpagedValidator(IAutenticationHelper autenticationHelper) : base(autenticationHelper) { 
        }
        public override IList<string> RequiredPermissions => new List<string> { "user-create", "crear-anuncio" };
    }
}
