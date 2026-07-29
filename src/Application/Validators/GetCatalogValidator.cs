using Application.Commands;
using Application.Services.Validaciones;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Validators
{
    class GetCatalogValidator : Validator<GetCatalog>
    {
        public GetCatalogValidator(IAutenticationHelper autenticationHelper): base(autenticationHelper)
        {
            RuleFor(x => x.Type).NotEmpty();
        }
        public override IList<string> RequiredPermissions => new List<string>();
    }
}
