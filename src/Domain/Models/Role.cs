using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain.Models
{
   public class Role:IEntity
    {

        public static int IdRolUsuarioRecibo= 2;
        public static int IdRolAdministracionSistema = 1;


        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public IList<RolePermission> Permissions { get; set; }
        public bool IsAssignable { get; set; }

        /// <summary>NULL = Roles de plataforma; NOT NULL = Roles local del Tenants.</summary>
        public int? TenantId { get; set; }
        public Tenant Tenant { get; set; }

        public void CreateRolePermissions(IList<int> permisosLista) {
            this.Permissions = new List<RolePermission>();
            if (permisosLista == null) return;
            foreach (var item in permisosLista.Distinct())
            {
                if (item <= 0) continue;
                Permissions.Add(new RolePermission { PermissionId = item, RoleId = this.Id, Role = this });
            }
        }
        public void SetCreatedAt() {
            this.CreatedAt = DateTime.Now;
        }

        public void ClearPermissions()
        {
            this.Permissions = new List<RolePermission>();
        }

        public void Update(string nombre, string descripcion, IList<int> permisos)
        {
            this.Name=nombre ;
            this.Description = descripcion;
            this.UpdatedAt = DateTime.Now;
            this.CreateRolePermissions(permisos);
        }
    }
}
