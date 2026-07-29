using Application.Common;
using Domain.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.AppUser
{
    public class TemporaryCode : IAppMessage
    {
        public string AccessIdentifier { get; set; }
    }
}
