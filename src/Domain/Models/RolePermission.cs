using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class RolePermission:IEntity
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public int PermissionId { get; set; }
        public Permission Permission { get; set; } 
    }
}
