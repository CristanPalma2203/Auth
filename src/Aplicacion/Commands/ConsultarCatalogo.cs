using Aplicacion.Common;
using Dominio.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Commands
{
    public class ConsultarCatalogo: IAppMessage
    {
        public string Type { get; set; }
        public int ParentId { get; set; }
    }
}
 
