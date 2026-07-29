using Application.Commands.AppUser;
using Application.Dtos;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CommandHandlers.AppUser
{
    class ChangePasswordHandler : AbstractHandler<ChangePassword>
    {
        private readonly IAppUserRepository appUserRepository;

        public ChangePasswordHandler(IAppUserRepository appUserRepository)
        {
            this.appUserRepository = appUserRepository;
        }
        public override IResponse Handle(ChangePassword message)
        {
            var dbUser = appUserRepository.GetById(message.Id);
            dbUser.AccessIdentifier = message.AccessIdentifier;
            dbUser.OwnerChangesPassword(message.Password);
            appUserRepository.Update(dbUser.Id, dbUser);
            return new OkResponse();
        }
    }
}
