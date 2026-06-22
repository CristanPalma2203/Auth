using Aplicacion.Common;
using Dominio.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Commands.Importador
{
    public class ConsultarImportador: IAppMessage
    {
        public int IdImportador { get; set; }
    }
}
