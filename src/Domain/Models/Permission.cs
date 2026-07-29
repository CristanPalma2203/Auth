using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class Permission: IEntity
    {
        public static int idPermisoAdministracion = 1;
        public static int idPermisoAdminitracionImportador = 21;
        public static int idPermisoProductos = 22;


        public static string codigoPermisoAdministracion = "administration";
        public static List<int> accesosParaAdmin= new List<int> { idPermisoAdministracion,1, 2,3,4,5,6,7,8,9,10,12,11,12,13,14,15,16,17,18,19,20 };

        public int Id { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int? ParentPermissionId { get; set; }
        public bool IsMenu { get; set; }
        public string Icon { get; set; }
        public int SortOrder { get; set; }
        public bool IsAssignable { get; set; }
        public bool HasChildren { get; set; }
    }
}
