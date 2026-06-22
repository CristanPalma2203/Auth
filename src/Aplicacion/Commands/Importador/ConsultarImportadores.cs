using Aplicacion.Common;
using Dominio.Repositories.Extenciones;
using Dominio.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Commands.Importador
{
   public class ConsultarImportadores: QueryStringParameters, IAppMessage
    {
        public string Consulta { get; set; }
        public string identificador { get; set; }
        public string nombre { get; set; }
    }
}
