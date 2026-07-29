using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dtos.AppUser
{
    public class UserLoginDto: UserBaseDto, IResponse
    {
        public string Token { get; set; }

    }
}
