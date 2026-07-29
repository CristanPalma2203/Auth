using Application.Commands.ExternalUser;
using Application.Services.Validaciones;
using Domain.Repositories;
using FluentValidation;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Application.Validators.ExternalUser
{
    public class ManageAccessValidator : Validator<ManageAccess>
    {
        private readonly IExternalUserRepository externalUserRepository;

        public ManageAccessValidator(IAutenticationHelper autenticationHelper, IExternalUserRepository externalUserRepository) : base(autenticationHelper)
        {
            RuleFor(x => x.ExternalUserId).NotEmpty().Must(c => ExternalUserHasAccess(c))
                .WithMessage("No se le ha creado un usuario al importador");
            RuleFor(x => x.Accesos).NotEmpty().Must(list => list.Count > 0)
            .WithMessage("Debes incluir al menos un item");
            RuleFor(x => x).NotEmpty().Must(c => PuedeSolicitarAcceso(c)).WithMessage("No puedes aprobar accesos que no han sidoSolicitados");
            this.externalUserRepository = externalUserRepository;
        }
        private bool ExternalUserHasAccess(int externalUserId)
        {
            var importado = externalUserRepository.GetById(externalUserId);
            return importado.AccessApproved;
        }
        public bool PuedeSolicitarAcceso(ManageAccess gestionAcceso)
        {
            var externalUser = externalUserRepository.GetByIdConDependencias(gestionAcceso.ExternalUserId);
            if (externalUser == null) return true;
            return false;
        }

        public override IList<string> RequiredPermissions => new List<string> { "manage-external-user-access", "manage-external-user-access" };
    }
}
