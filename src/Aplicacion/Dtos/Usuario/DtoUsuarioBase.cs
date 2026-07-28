using System;
using System.Collections.Generic;

namespace Aplicacion.Dtos.Usuario
{
    public class DtoUsuarioBase
    {
        public int? DepartamentoId { get; set; }
        public string DepartamentoNombre { get; set; }
        public IList<DtoRol> Roles { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }

        public string AccessIdentifier { get; set; }
        public bool IsActive { get; set; }
        public bool MustChangePassword { get; set; }

        public string TemporaryCode { get; set; }
        public string UserType { get; set; }
        public DateTime? RegisteredAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<DtoUsuarioRegional> UsuarioRegional { get; set; }
        public ICollection<DtoUsuarioArea> UsuarioArea { get; set; }
        public int? TenantId { get; set; }
        public string TenantCodigo { get; set; }

    }

}
