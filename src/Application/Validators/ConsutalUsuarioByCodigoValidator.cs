using Application.Commands.AppUser;
using Application.Services.Validaciones;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Validators
{
     class GetUserByCodeValidator : Validator<GetUserByCode>
    {
        public GetUserByCodeValidator(IAutenticationHelper autenticationHelper) : base(autenticationHelper)
        {
            RuleFor(x => x.TemporaryCode).NotEmpty();
        }
        public override IList<string> RequiredPermissions => new List<string>();
    }
}
