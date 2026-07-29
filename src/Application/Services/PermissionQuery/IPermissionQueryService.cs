using Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.PermissionQuery
{
    public interface IPermissionQueryService
    {
        IEnumerable<PermissionDto> Estructurar(IEnumerable<PermissionDto> permissions);
    }
}
