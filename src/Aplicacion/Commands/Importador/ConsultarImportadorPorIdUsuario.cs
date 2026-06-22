using Aplicacion.Common;
using Dominio.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Commands.Importador
{
    public class ConsultarImportadorPorIdUsuario : IAppMessage
    {
        public int IdUsuario { get; set; }
    }
}
