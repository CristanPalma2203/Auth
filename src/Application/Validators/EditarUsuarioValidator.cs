using Application.Commands.AppUser;
using Application.Services.Validaciones;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Validators
{
    public class EditUserValidator : Validator<EditUser>
    {
        public EditUserValidator(IAutenticationHelper autenticationHelper) : base(autenticationHelper) { }
        public override IList<string> RequiredPermissions => new List<string> { "user-edit" };
    }
}
