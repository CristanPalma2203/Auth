using Application.Commands.AppUser;
using Application.Dtos;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CommandHandlers.AppUser
{
    public class EditPasswordHandler : AbstractHandler<EditPassword>
    {
        private readonly IAppUserRepository appUserRepository;

        public EditPasswordHandler(IAppUserRepository appUserRepository)
        {
            this.appUserRepository = appUserRepository;
        }
        public override IResponse Handle(EditPassword message)
        {
            var dbUser = appUserRepository.GetById(message.Id);
            dbUser.OwnerChangesPassword(message.Password);
            appUserRepository.Update(dbUser.Id, dbUser);
            return new OkResponse();
        }
    }
}
