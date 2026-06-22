using Aplicacion.Common;
using Dominio.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Commands.Importador
{
    public class ConsultarImportadorPorIdentificador: IAppMessage
    {
        public string IdImportador { get; set; }
    }
}
