using Aplicacion.Common;
using Dominio.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Commands
{
    public class ConsultarCatalogo: IAppMessage
    {
        public string Tipo { get; set; }
        public int IdPadre { get; set; }
    }
}
 
