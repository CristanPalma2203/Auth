using Application.Commands.AppUser;
using Application.Services.Validaciones;
using FluentValidation;
using System.Collections.Generic;

namespace Application.Validators
{
    public class DeleteUserValidator : Validator<DeleteUser>
    {
        public DeleteUserValidator(IAutenticationHelper autenticationHelper) : base(autenticationHelper)
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }

        public override IList<string> RequiredPermissions => new List<string> { "user-edit" };
    }
}
