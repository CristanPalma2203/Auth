using Application.Common;
using Domain.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.ExternalUser
{
    public class GetExternalUser: IAppMessage
    {
        public int IdImportador { get; set; }
    }
}
