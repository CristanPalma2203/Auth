using Application.Common;
using Domain.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.ExternalUser
{
    public class ResetExternalUserPassword: IAppMessage
    {
        public string AppUser { get; set; }
        public string Email { get; set; }
    }
}
