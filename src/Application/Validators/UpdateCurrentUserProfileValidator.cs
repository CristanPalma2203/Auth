using Application.Commands.AppUser;
using Application.Services.Validaciones;
using FluentValidation;
using System.Collections.Generic;

namespace Application.Validators
{
    public class UpdateCurrentUserProfileValidator : Validator<UpdateCurrentUserProfile>
    {
        public UpdateCurrentUserProfileValidator(IAutenticationHelper autenticationHelper)
            : base(autenticationHelper)
        {
            RuleFor(x => x.ProfileFileId)
                .Must(id => !id.HasValue || id.Value > 0)
                .WithMessage("Foto de perfil inválida");
        }

        public override IList<string> RequiredPermissions => new List<string>();
    }
}
