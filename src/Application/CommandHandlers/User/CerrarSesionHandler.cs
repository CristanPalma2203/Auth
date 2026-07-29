using Application.Commands.AppUser;
using Application.Dtos;
using Domain.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CommandHandlers.AppUser
{
    public class SignOutHandler : AbstractHandler<SignOut>
    {
        private readonly ITokenService tokenService;

        public SignOutHandler(ITokenService tokenService) {
            this.tokenService = tokenService;
        }
        public override IResponse Handle(SignOut message)
        {
            this.tokenService.RemoveToken();
            return new OkResponse();
        }
    }
}
