using Application.Common;
using Application.Dtos.AppUser;
using Application.Services;
using Domain.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.AppUser
{
    public class RegisterUser : IAppMessage
    {
        public UserDto AppUser { get; set; }
    }
}
