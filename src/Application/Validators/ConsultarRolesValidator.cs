using Application.Commands.Role;
using Application.Services.Validaciones;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Validators
{
    public class ListRolesValidator : Validator<ListRoles>
    {
        public ListRolesValidator(IAutenticationHelper autenticationHelper) : base(autenticationHelper) { }
        public override IList<string> RequiredPermissions => new List<string> { "role-list", "roles" };
    }
}
