using Application.Commands.ExternalUser;
using Application.Services.Validaciones;
using Domain.Repositories;
using FluentValidation;
using System.Collections.Generic;

namespace Application.Validators.ExternalUser
{
    public class RejectAccessRequestValidator : Validator<RejectAccessRequest>
    {
        private readonly IExternalUserRepository externalUserRepository;

        public RejectAccessRequestValidator(IAutenticationHelper autenticationHelper, IExternalUserRepository externalUserRepository) : base(autenticationHelper)
        {
            RuleFor(x => x.ExternalUserId).NotEmpty().Must(c=>ExternalUserRequestExists(c)).WithMessage("No es posible gestinar esta solictud por que ya fue aprobada o no existe");
            RuleFor(x => x.Motivo).NotEmpty().WithMessage("Comment Obligatorio Al Rechazar");
            this.externalUserRepository = externalUserRepository;
        }

        public bool ExternalUserRequestExists(int externalUserId) {
            var externalUser = externalUserRepository.GetById(externalUserId);
            if (externalUser == null) return false;
            if (externalUser.AccessApproved) return false;
            return true;
        }
        public override IList<string> RequiredPermissions => new List<string> { "manage-external-user", "manage-external-user" };
    }
}
