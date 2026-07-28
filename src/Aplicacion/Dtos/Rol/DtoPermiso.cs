using System.Collections.Generic;

namespace Aplicacion.Dtos
{
    public class DtoPermiso {
        public int Id { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int? ParentPermissionId { get; set; }
        public bool IsMenu { get; set; }
        public string Icon { get; set; }
        public int SortOrder { get; set; }
        public bool HasChildren { get; set; }
        public IEnumerable<DtoPermiso> Hijos { get; set; }
    }
}
