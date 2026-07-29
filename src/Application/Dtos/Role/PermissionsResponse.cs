using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dtos
{
    public class PermissionsResponse: IResponse
    {
        public IEnumerable<PermissionDto> Permissions { get; set; }
    }
}
