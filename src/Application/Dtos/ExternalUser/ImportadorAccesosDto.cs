using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dtos.ExternalUser
{
    public class ExternalUserAccessDto 
    {
        public int Id { get; set; }
        public int AccesoId { get; set; }
        public bool IsActive { get; set; }
        public RoleDto Acceso { get; set; }
    }
}
