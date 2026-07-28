using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Models
{
   public class Rol:IEntity
    {

        public static int IdRolUsuarioRecibo= 2;
        public static int IdRolAdministracionSistema = 1;


        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public IList<RolPermiso> Permisos { get; set; }
        public bool IsAssignable { get; set; }

        /// <summary>NULL = rol de plataforma; NOT NULL = rol local del tenant.</summary>
        public int? TenantId { get; set; }
        public Tenant Tenant { get; set; }

        public void CrearRolPermiso(IList<int> permisosLista) {
            this.Permisos = new List<RolPermiso>();
            foreach (var item in permisosLista)
            {
                Permisos.Add(new RolPermiso { PermisoId=item,Rol=this});
            }
           
        }
        public void setFechaCreacion() {
            this.CreatedAt = DateTime.Now;
        }

        public void limpiaPermisos()
        {
            this.Permisos = new List<RolPermiso>();
        }

        public void actualizar(string nombre, string descripcion, IList<int> permisos)
        {
            this.Name=nombre ;
            this.Description = descripcion;
            this.UpdatedAt = DateTime.Now;
            this.CrearRolPermiso(permisos);
        }
    }
}
