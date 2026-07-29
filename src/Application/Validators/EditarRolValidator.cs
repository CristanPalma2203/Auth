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

            RuleFor(x => x.Role).NotEmpty().Must(c => roleRepository.Filter(new Func<Domain.Models.Role, bool>(p => p.Name == c.Name && p.Id!=c.Id)).Count() == 0)
               .WithMessage("Ya existe un Roles con el mismo nombre"); ;
            RuleFor(x => x.Role.Description).NotEmpty();
            RuleFor(x => x.Role.PermissionIds).NotEmpty();
        }
        public override IList<string> RequiredPermissions => new List<string> { "role-edit" };

    }
}
