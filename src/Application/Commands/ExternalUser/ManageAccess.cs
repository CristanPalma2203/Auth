using Application.Common;
using Domain.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.ExternalUser
{
    public class ManageAccess : IAppMessage
    {
        public int ExternalUserId { get; set; }
        public List<int> Accesos { get; set; }
    }
}
