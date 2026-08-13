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
        /* Legacy: ya no filtrar por IDs fijos — platform admin recibe todos vía GetAllPermissionDtos */
        public static List<int> accesosParaAdmin = new List<int>();

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
