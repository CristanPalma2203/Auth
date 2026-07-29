using Application.Commands.AppUser;
using Application.Services.Validaciones;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Validators
{
    class GetUserValidator : Validator<GetUser>
    {
        public GetUserValidator(IAutenticationHelper autenticationHelper) : base(autenticationHelper)
        {
            RuleFor(x => x.Id).NotEmpty();
        }
        public override IList<string> RequiredPermissions => new List<string> { "user-edit", "user-view", "user-edit-staff" };
    }
}