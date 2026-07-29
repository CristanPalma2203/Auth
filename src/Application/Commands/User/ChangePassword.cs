using Application.Common;
using Domain.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.AppUser
{
   public class ChangePassword: IAppMessage
    {
 
        public string Password { get; set; }
        public string AccessIdentifier { get; set; }
        public int Id { get; set; }

    }
}
