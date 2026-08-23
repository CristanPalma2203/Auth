using Application.Commands.Role;
using Application.Services.Validaciones;
using Domain.Repositories;
using Domain.Service;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.Validators
{
    public class CreateRoleValidator : Validator<CreateRole>
    {
        public CreateRoleValidator(
            IRoleRepository roleRepository,
            IAutenticationHelper autenticationHelper,
            ITenantContext tenantContext) : base(autenticationHelper)
        {
            RuleFor(x => x.Role.Name).NotEmpty();
            RuleFor(x => x)
                .Must(cmd => cmd.Role == null || string.IsNullOrWhiteSpace(cmd.Role.Name)
                    || roleRepository.Filter(p =>
                        p.Name == cmd.Role.Name
                        && p.TenantId == cmd.Role.TenantId).Count() == 0)
                .WithMessage("Ya existe un rol con el mismo nombre en esta empresa"); 
            RuleFor(x => x.Role.Description).NotEmpty();
            RuleFor(x => x.Role.PermissionIds).NotEmpty();
            When(_ => tenantContext.IsPlatformAdmin, () =>
                RuleFor(x => x.Role.TenantId).NotNull()
                    .WithMessage("Debe seleccionar la empresa del rol"));
        }
        public override IList<string> RequiredPermissions => new List<string> { "role-create" };
    }
}
