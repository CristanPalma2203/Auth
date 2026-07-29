using Application.Common;
using Domain.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.ExternalUser
{
    public class RejectAccessRequest: IAppMessage
    {
        public int ExternalUserId { get; set; }
        public string Motivo { get; set; }
    }
}
