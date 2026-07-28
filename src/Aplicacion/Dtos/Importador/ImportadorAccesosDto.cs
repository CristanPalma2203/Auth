using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Dtos.Importador
{
    public class ImportadorAccesosDto 
    {
        public int Id { get; set; }
        public int AccesoId { get; set; }
        public bool IsActive { get; set; }
        public DtoRol Acceso { get; set; }
    }
}
