using System;
using System.Collections.Generic;

namespace Application.Dtos.AppUser
{
    public class UserBaseDto
    {
        public int? DepartmentId { get; set; }
        public string DepartamentoNombre { get; set; }
        public IList<RoleDto> Roles { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }

        public string AccessIdentifier { get; set; }
        public string Dui { get; set; }
        public string Nit { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public bool MustChangePassword { get; set; }

        public string TemporaryCode { get; set; }
        public string UserType { get; set; }
        public DateTime? RegisteredAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? TenantId { get; set; }
        public string TenantCodigo { get; set; }
        public int? ProfileFileId { get; set; }

    }

}
