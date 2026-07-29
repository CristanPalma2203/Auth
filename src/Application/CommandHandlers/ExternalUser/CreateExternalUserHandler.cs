using Application.Commands.ExternalUser;
using Application.Dtos;
using Application.Dtos.ExternalUser;
using MapsterMapper;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CommandHandlers.ExternalUser
{
    public class CreateExternalUserHandler : AbstractHandler<CreateExternalUser>
    {
        private readonly IExternalUserRepository externalUserRepository;
        private readonly IMapper mapper;
        public CreateExternalUserHandler(IExternalUserRepository externalUserRepository, IMapper mapper)
        {
            this.externalUserRepository = externalUserRepository;
            this.mapper = mapper;
        }

        public override IResponse Handle(CreateExternalUser message)
        {
            var externalUser = mapper.Map<Domain.Models.ExternalUser>(message.ExternalUser);
            externalUser.EmailVerified = false;
            var importadorCreado = externalUserRepository.Create(externalUser);
            return mapper.Map<ExternalUserDto>(importadorCreado);
        }
    }
}
