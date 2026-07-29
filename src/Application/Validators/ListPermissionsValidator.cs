using Application.Commands;
using Application.Services.Validaciones;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Validators
{
    public class ListPermissionsValidator : Validator<ListPermissions>
    {
        public ListPermissionsValidator(IAutenticationHelper autenticationHelper):base(autenticationHelper) { }
        public override IList<string> RequiredPermissions => new List<string> { "role-create" };
    }
}
