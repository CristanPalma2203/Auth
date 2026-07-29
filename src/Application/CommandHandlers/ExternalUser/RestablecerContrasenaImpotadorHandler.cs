using Application.Commands.ExternalUser;
using Application.Dtos;
using Application.Helpers;
using Domain.Specifications;
using Domain.Helpers;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.CommandHandlers.ExternalUser
{
    public class ResetExternalUserPasswordHandler : AbstractHandler<ResetExternalUserPassword>
    {
        private readonly IExternalUserRepository externalUserRepository;
        private readonly IAppUserRepository appUserRepository;

        private readonly IEmailHelper correoHelper;

        public ResetExternalUserPasswordHandler(IExternalUserRepository externalUserRepository, IAppUserRepository appUserRepository, IEmailHelper correoHelper)
        {
            this.externalUserRepository = externalUserRepository;
            this.appUserRepository = appUserRepository;
            this.correoHelper = correoHelper;
        }


        public override IResponse Handle(ResetExternalUserPassword message)
        {
            var password = StringHelper.RandomString(7);
            var appUser = appUserRepository.Filter(new FindUserByIdentifier(message.AppUser)).FirstOrDefault();
            if (appUser != null)
            {
                var externalUser = externalUserRepository.Filter(new FindExternalUserByEmailIdentifier(message.Email, message.AppUser)).FirstOrDefault();

                if (externalUser != null)
                {
                    appUser.ResetExternalUserPassword(password);
                    appUserRepository.Update(appUser.Id, appUser);
                    correoHelper.SendUserCreatedEmail(externalUser.Identifier, password, externalUser.Email);
                    return new OkResponse();
                }
            }
            return new OkResponse();
        }
    }
}
