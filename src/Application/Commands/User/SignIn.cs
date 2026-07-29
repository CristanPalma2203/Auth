using Application.Common;
using Domain.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.AppUser
{
    public class SignIn: IAppMessage
    {
        public string AppUser { get; set; }
        public string Password { get; set; }

    }
}
