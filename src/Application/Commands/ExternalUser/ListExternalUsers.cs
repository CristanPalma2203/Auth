using Application.Common;
using Domain.Repositories.Extenciones;
using Domain.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.ExternalUser
{
   public class ListExternalUsers: QueryStringParameters, IAppMessage
    {
        public string Consulta { get; set; }
        public string identifier { get; set; }
        public string nombre { get; set; }
    }
}
