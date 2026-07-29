using Application.Common;
using Application.Dtos;
using Application.Dtos.AppUser;
using Domain.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.AppUser
{
    public class EditUser : IAppMessage
    {
        public UserDto AppUser { get; set; }
    }
}
