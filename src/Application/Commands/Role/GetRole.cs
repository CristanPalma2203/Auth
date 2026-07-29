using Application.Common;
using Domain.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Role
{
    public class GetRole : IAppMessage
    {
        public int id { get; set; }
    }
}
