using Application.Common;
using Domain.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.ExternalUser
{
    public class VerifyEmail: IAppMessage
    {
        public string Token { get; set; }
    }
}
