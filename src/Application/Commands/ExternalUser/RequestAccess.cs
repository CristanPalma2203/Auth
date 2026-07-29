using Application.Common;
using Application.Dtos.ExternalUser;
using Domain.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.ExternalUser
{
    public class RequestAccess: IAppMessage
    {
       public ExternalUserDto ExternalUser { get; set; }
    }
}
