using Application.Commands.ExternalUser;
using Application.Dtos;
using MapsterMapper;
using Domain.Models;
using Domain.Helpers;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Application.CommandHandlers.ExternalUser
{
    public class RequestAccessHandler : AbstractHandler<RequestAccess>
    {
        private readonly IExternalUserRepository externalUserRepository;
        private readonly IMapper mapper;
        private readonly IEmailHelper correoHelper;

        public RequestAccessHandler(IExternalUserRepository externalUserRepository, IMapper mapper, IEmailHelper correoHelper)
        {
            this.externalUserRepository = externalUserRepository;
            this.mapper = mapper;
            this.correoHelper = correoHelper;
        }
        public override IResponse Handle(RequestAccess message)
        {
            
            var externalUser = mapper.Map<Domain.Models.ExternalUser>(message.ExternalUser);
            externalUser.RequestAccess();
            var impotadorBusquedad = externalUserRepository.Filter(new Func<Domain.Models.ExternalUser, bool>(c => c.Identifier == message.ExternalUser.Identifier)).FirstOrDefault();
            if (impotadorBusquedad == null)
            {
                externalUserRepository.Create(externalUser);
            }
            else {
                externalUserRepository.Update(impotadorBusquedad.Id, externalUser);
            }
            
            correoHelper.SendVerificationEmail(externalUser.Email, externalUser.VerificationToken);

            return new OkResponse();
        }
    }
}
