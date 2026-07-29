using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class UserRole:  IEntity
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public int UserId { get; set; }
        public AppUser User { get; set; }

    }
}
