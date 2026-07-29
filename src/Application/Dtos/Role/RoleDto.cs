using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dtos
{
    public class RoleDto:IResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }
        public IList<int> PermissionIds { get; set; }
        public IList<PermissionDto> PermissionsWithMetadata { get; set; }
    }
}
