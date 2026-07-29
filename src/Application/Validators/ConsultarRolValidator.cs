using Application.Commands.Role;
using Application.Services.Validaciones;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Validators
{
    public class GetRoleValidator : Validator<GetRole>
    {
        public GetRoleValidator(IAutenticationHelper autenticationHelper) : base(autenticationHelper)
        {
            RuleFor(x => x.id).NotEmpty();
        }
        public override IList<string> RequiredPermissions => new List<string> { "role-edit", "role-view" };
    }
}
