using Application.Common;
using Domain.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.AppUser
{
    public class EditPassword : IAppMessage
    {

        public string Password { get; set; }
        public int Id { get; set; }

    }
}
