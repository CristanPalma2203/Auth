using Application.Commands.Role;
using Application.Services.Validaciones;
using Domain.Repositories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.Validators
{
    public class EditRoleValidator : Validator<EditRole>
    {
        public EditRoleValidator(IRoleRepository roleRepository, IAutenticationHelper autenticationHelper):base(autenticationHelper)
        {

            RuleFor(x => x)
                .Must(cmd => cmd.Role != null && roleRepository.Filter(p =>
                    p.Name == cmd.Role.Name
                    && p.Id != cmd.Id
                    && p.TenantId == cmd.Role.TenantId).Count() == 0)
                .WithMessage("Ya existe un rol con el mismo nombre en esta empresa");
            RuleFor(x => x.Role).NotNull();
            When(x => x.Role != null, () =>
            {
                RuleFor(x => x.Role.Description).NotEmpty();
                RuleFor(x => x.Role.PermissionIds).NotEmpty();
            });
        }
        public override IList<string> RequiredPermissions => new List<string> { "role-edit" };

    }
}
