using Application.Commands.ExternalUser;
using Application.Services.Validaciones;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Validators.ExternalUser
{
    public class GetExternalUserByUserIdValidator : Validator<GetExternalUserByUserId>
    {
        public GetExternalUserByUserIdValidator(IAutenticationHelper autenticationHelper) : base(autenticationHelper)
        {
        }

        public override IList<string> RequiredPermissions => new List<string> {   };
    }
}
