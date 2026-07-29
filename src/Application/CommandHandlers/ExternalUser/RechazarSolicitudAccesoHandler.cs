using Application.Commands.ExternalUser;
using Application.Dtos;
using Domain.Helpers;
using Domain.Repositories;
using Domain.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CommandHandlers.ExternalUser
{
    public class RejectAccessRequestHandler : AbstractHandler<RejectAccessRequest>
    {
        private readonly IExternalUserRepository externalUserRepository;
        private readonly ITokenService tokenService;
        private readonly IEmailHelper correoHelper;

        public RejectAccessRequestHandler(IExternalUserRepository externalUserRepository, ITokenService tokenService, IEmailHelper correoHelper)
        {
            this.externalUserRepository = externalUserRepository;
            this.tokenService = tokenService;
            this.correoHelper = correoHelper;
        }

        public override IResponse Handle(RejectAccessRequest message)
        {
            var externalUser = externalUserRepository.GetById(message.ExternalUserId);
            externalUser.DenegarAcceso(tokenService.GetUserId(), message.Motivo);
            correoHelper.SendAccessDeniedEmail(externalUser.Email, message.Motivo);
            externalUserRepository.Update(externalUser.Id, externalUser);
            return new OkResponse();
        }
    }
}
