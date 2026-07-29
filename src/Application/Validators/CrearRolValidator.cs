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
    public class CreateRoleValidator : Validator<CreateRole>
    {
        public CreateRoleValidator(IRoleRepository roleRepository,IAutenticationHelper autenticationHelper):base(autenticationHelper)
        {
            RuleFor(x => x.Role.Name).NotEmpty().Must(c => roleRepository.Filter(new Func<Domain.Models.Role, bool>(p => p.Name == c)).Count() == 0)
                .WithMessage("Ya existe un Roles con el mismo nombre"); 
            RuleFor(x => x.Role.Description).NotEmpty();
            RuleFor(x => x.Role.PermissionIds).NotEmpty();
        }
        public override IList<string> RequiredPermissions => new List<string> { "role-create" };
    }
}
