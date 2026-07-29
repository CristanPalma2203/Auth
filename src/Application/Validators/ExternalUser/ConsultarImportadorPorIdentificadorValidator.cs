using Application.Commands.ExternalUser;
using Application.Services.Validaciones;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Validators.ExternalUser
{
    public class GetExternalUserValidator : Validator<GetExternalUser>
    {
        public GetExternalUserValidator(IAutenticationHelper autenticationHelper) : base(autenticationHelper)
        {
        }

        public override IList<string> RequiredPermissions => new List<string> { };
    }
}
