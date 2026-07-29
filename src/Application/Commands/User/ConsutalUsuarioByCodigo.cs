using Application.Common;
using Domain.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.AppUser
{
    public class GetUserByCode : IAppMessage
    {
        public string TemporaryCode { get; set; }
        public string Email { get; set; }
    }
}
