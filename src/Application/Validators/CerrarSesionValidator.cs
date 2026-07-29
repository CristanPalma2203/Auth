using Application.Commands.AppUser;
using Application.Services.Validaciones;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Validators
{
   public class SignOutValidator : Validator<SignOut>
    {
        public SignOutValidator(IAutenticationHelper autenticationHelper) : base(autenticationHelper)
        {
        }
        public override IList<string> RequiredPermissions => new List<string>();
    }
}
